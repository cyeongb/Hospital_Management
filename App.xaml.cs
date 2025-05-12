using System;
using System.Windows;

namespace HospitalManagementSystem
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 애플리케이션 시작 시 초기화 코드
            // 예: 데이터베이스 연결 확인, 로깅 설정 등

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // 예외 처리 로직
            Exception ex = e.ExceptionObject as Exception;
            MessageBox.Show($"예상치 못한 오류가 발생했습니다: {ex?.Message}",
                           "오류", MessageBoxButton.OK, MessageBoxImage.Error);

            // 로깅 로직을 추가할 수 있음
        }
    }
}