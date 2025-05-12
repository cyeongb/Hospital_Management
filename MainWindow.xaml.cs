using System;
using System.Windows;
using System.Windows.Threading;
using HospitalManagementSystem.ViewModels;
using MahApps.Metro.Controls;

namespace HospitalManagementSystem
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly DispatcherTimer _timer;
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            // 뷰모델 초기화
            _viewModel = new MainViewModel();
            DataContext = _viewModel;

            // 타이머 설정 (현재 시간 표시용)
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();

            // 초기 시간 설정
            UpdateCurrentTime();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateCurrentTime();
        }

        private void UpdateCurrentTime()
        {
            CurrentDateTimeText.Text = DateTime.Now.ToString("yyyy년 MM월 dd일 HH:mm:ss");
        }

        protected override void OnClosed(EventArgs e)
        {
            // 타이머 정리
            _timer.Stop();
            _timer.Tick -= Timer_Tick;

            base.OnClosed(e);
        }
    }
}