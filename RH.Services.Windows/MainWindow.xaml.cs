using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Research.Science.Data.Imperative;
using Microsoft.Win32;
using RH.EntityFramework.Repositories.Wind;
using RH.EntityFramework.Shared.DbContexts;
using RH.EntityFramework.Shared.Entities;

namespace RH.Services.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker worker;
        private string filePath = "";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                txtPath.Text = fileDialog.FileName;
            }
        }

        private void Worker_DoWorkAsync(object sender, DoWorkEventArgs e)
        {

            {

                var connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
                WeatherDbContext _db = new WeatherDbContext(new DbContextOptionsBuilder().UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)).Options);
                GfsWindRepository _gfsRepository = new GfsWindRepository(_db);
                var fileName = filePath;
                var dataset = Microsoft.Research.Science.Data.DataSet.Open(fileName);
                SetProgress(0, "پردازش ابعاد: طول، عرض ، زمان");
                int[] TimeDim = dataset.GetData<int[]>(4);
                Single[] latDim = dataset.GetData<Single[]>(1);
                Single[] longDim = dataset.GetData<Single[]>(2);
                SetProgress(2,"نرمالسازی دما");

                var temprature = new NetCDFData<Int16>
                {
                    Data = dataset.GetData<Int16[,,,]>("t"),
                    factor =
                        (double)dataset.Variables.All.FirstOrDefault(x => x.Name == "t").Metadata.AsDictionary()[
                            "scale_factor"],
                    offset =
                        (double)dataset.Variables.All.FirstOrDefault(x => x.Name == "t").Metadata.AsDictionary()[
                            "add_offset"]
                };
                SetProgress(4,"نرمالسازی فشار");

                var GeoPotential = new NetCDFData<Int16>
                {
                    Data = dataset.GetData<Int16[,,,]>("z"),
                    factor =
                        (double)dataset.Variables.All.FirstOrDefault(x => x.Name == "z").Metadata.AsDictionary()[
                            "scale_factor"],
                    offset =
                        (double)dataset.Variables.All.FirstOrDefault(x => x.Name == "z").Metadata.AsDictionary()[
                            "add_offset"]
                };
                SetProgress(6, "نرمالسازی رطوبت نسبی");

                var rh = new NetCDFData<Int16>
                {
                    Data = dataset.GetData<Int16[,,,]>("r"),
                    factor =
                        (double)dataset.Variables.All.FirstOrDefault(x => x.Name == "r").Metadata.AsDictionary()[
                            "scale_factor"],
                    offset =
                        (double)dataset.Variables.All.FirstOrDefault(x => x.Name == "r").Metadata.AsDictionary()[
                            "add_offset"]
                };
                SetProgress(8, "نرمالسازی سرعت باد");

                var u = new NetCDFData<Int16>
                {
                    Data = dataset.GetData<Int16[,,,]>("u"),
                    factor =
                        (double)dataset.Variables.All.FirstOrDefault(x => x.Name == "u").Metadata.AsDictionary()[
                            "scale_factor"],
                    offset =
                        (double)dataset.Variables.All.FirstOrDefault(x => x.Name == "u").Metadata.AsDictionary()[
                            "add_offset"]
                };
                SetProgress(10, "نرمالسازی حهت باد");
                var v = new NetCDFData<Int16>
                {
                    Data = dataset.GetData<Int16[,,,]>("v"),
                    factor =
                        (double)dataset.Variables.All.FirstOrDefault(x => x.Name == "v").Metadata.AsDictionary()[
                            "scale_factor"],
                    offset =
                        (double)dataset.Variables.All.FirstOrDefault(x => x.Name == "v").Metadata.AsDictionary()[
                            "add_offset"]
                };
                



                var timeMax = TimeDim.Length;
                var longMax = longDim.Length;
                var latMax = latDim.Length;
                double change = 12;
                var dimInMem = new Dictionary<int, WindDimension>();
                var prevlat0 = -1;
                var prevLong0 = -1;
                var dimInterval = 8.0 / latMax;
                SetProgress(12, $"انتقال موقعیت های مکانی. {0} از {latMax*longMax}");
                for (int iLatitude = 0; iLatitude < latMax; iLatitude++)
                {
                    change += dimInterval;
                    //SetProgress((int)change);
                    SetProgress((int)change, $"انتقال موقعیت های مکانی. {iLatitude*longMax} از {latMax * longMax}");
                    int lat = (int)latDim[iLatitude];
                    if (lat == prevlat0)
                        continue;
                    for (int iLongitude = 0; iLongitude < longMax; iLongitude++)
                    {
                        var lon = (int)longDim[iLongitude];
                        if (lat == prevlat0)
                            continue;
                        var dimension = _db.WindDimensions.FirstOrDefault(d => Math.Abs(d.X - lat) <= 0.5 && Math.Abs(d.Y - lon) <= 0.5);
                        if (dimension == null)
                        {
                            dimension = new WindDimension()
                            {
                                X = lat,
                                Y = lon,
                                IsActive = false
                            };
                            _db.WindDimensions.Add(dimension);
                        }
                        dimInMem.Add(iLatitude * longMax + iLongitude, dimension);
                        prevLong0 = lon;
                    }
                    _db.SaveChanges();
                    prevlat0 = lat;

                }

                double interval = 80.0 / (double)(timeMax * latMax);

                for (int iTime = 0; iTime < timeMax; iTime++)
                {
                    DateTime time = new DateTime(1900, 01, 01, 0, 0, 0).AddHours(TimeDim[iTime]);
                    var list = new List<string>();
                    var prevlat = -1;
                    for (int iLatitude = 0; iLatitude < latMax; iLatitude++)
                    {
                        change += interval;
                        //SetProgress((int)change);
                        SetProgress((int)change, $"انتقال اطلاعات. {iTime*latMax*longMax+iLatitude*longMax} از {latMax * longMax*timeMax}");
                        int lat = (int)latDim[iLatitude];
                        if (lat == prevlat)
                            continue;
                        var prevLong = -1;
                        for (int iLongitude = 0; iLongitude < longMax; iLongitude++)
                        {
                            // write output data
                            var lon = (int)longDim[iLongitude];
                            if (lon == prevLong)
                                continue;
                            double t = temprature.Data[iTime, 0, iLongitude, iLatitude] * temprature.factor +
                                       temprature.offset;
                            double z = GeoPotential.Data[iTime, 0, iLongitude, iLatitude] * GeoPotential.factor +
                                       GeoPotential.offset;
                            double r = rh.Data[iTime, 0, iLongitude, iLatitude] * rh.factor + rh.offset;
                            double ud = u.Data[iTime, 0, iLongitude, iLatitude] * u.factor + u.offset;
                            double vd = v.Data[iTime, 0, iLongitude, iLatitude] * v.factor + v.offset;

                            _db.GfsForecasts.Add(new EntityFramework.Shared.Entities.GfsForecast()
                            {
                                WindDimensionId = dimInMem[iLatitude * longMax + iLongitude].Id,
                                DateTime = time,
                                ReferenceTime = time,
                                Temperature = t,
                                Rh = (int)(r * 100),
                                Pressure = z,
                                Wind = Math.Sqrt(Math.Pow(ud, 2) + Math.Pow(vd, 2)),
                                WindDirection = ((int)(180 + (180 / Math.PI) * Math.Atan2(vd, ud))) % 360
                            });

                            var item =
                                $"{DateTime.Now:yyyy-MM-dd HH:mm}\t{time:yyyy-MM-dd HH:mm}\t{lat}\t{lon}\t{t.ToString("F4")}\t{z.ToString("F4")}\t{r.ToString("F4")}\t{ud.ToString("F4")}\t{vd.ToString("F4")}";
                            list.Add(item);
                            prevLong = lon;
                        }
                        _db.SaveChanges();
                        _db.ChangeTracker.Clear();
                        prevlat = lat;

                    }

                    System.IO.File.AppendAllLines(
                        $"{fileName}.txt", list);

                }
            }
        }

        private void SetProgress(int percentage,string state)
        {
            try { Dispatcher.Invoke(new System.Action(() => { worker.ReportProgress(percentage, state); })); }
            catch { }
        }

        void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MyProgressBar.Value = e.ProgressPercentage;
            MyProgressstate.Text = (string)e.UserState;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            //MyProgressBar.Visibility = Visibility.Collapsed;
            //MyProgressLabel.Visibility = Visibility.Collapsed;
            gridResult.Visibility = Visibility.Collapsed;
            gridInput.IsEnabled = true;
            MessageBox.Show("پایان عملیات ...");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(txtPath.Text))
            {
                MessageBox.Show("فایل یافت نشد...");
                return;
            }
            filePath = txtPath.Text;
            //MyProgressBar.Visibility = Visibility.Visible; //Make Progressbar visible
            //MyProgressLabel.Visibility = Visibility.Visible; //Make TextBlock visible
            gridResult.Visibility = Visibility.Visible;
            gridInput.IsEnabled = false; //Disabling the button
            try
            {
                var dataset = Microsoft.Research.Science.Data.DataSet.Open(txtPath.Text);

                int[] TimeDim = dataset.GetData<int[]>(4);
                Single[] latDim = dataset.GetData<Single[]>(1);
                Single[] longDim = dataset.GetData<Single[]>(2);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ساختار فایل ورودی نامعتبر است...");
                //MyProgressBar.Visibility = Visibility.Collapsed;
                //MyProgressLabel.Visibility = Visibility.Collapsed;
                gridResult.Visibility = Visibility.Collapsed;
                gridInput.IsEnabled = true;
                return;
            }

            worker = new BackgroundWorker(); //Initializing the worker object
            worker.ProgressChanged += Worker_ProgressChanged; //Binding Worker_ProgressChanged method
            worker.DoWork += Worker_DoWorkAsync; //Binding Worker_DoWork method
            worker.WorkerReportsProgress = true; //telling the worker that it supports reporting progress
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted; //Binding worker_RunWorkerCompleted method
            worker.RunWorkerAsync(); //Executing the worker
        }
    }

    public class NetCDFData<T>
    {
        public T[,,,] Data { get; set; }

        public double factor { get; set; }

        public double offset { get; set; }


    }
}
