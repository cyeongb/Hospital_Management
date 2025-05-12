using System;
using System.Windows.Input;

namespace HospitalManagementSystem.Helpers
{
    /// <summary>
    /// MVVM 패턴에서 Command 패턴을 구현하기 위한 RelayCommand 클래스
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        /// <summary>
        /// RelayCommand 생성자
        /// </summary>
        /// <param name="execute">명령이 실행될 때 호출되는 액션</param>
        /// <param name="canExecute">명령의 실행 가능 여부를 결정하는 조건부 (선택적)</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// 명령의 실행 가능 상태가 변경되었을 때 발생하는 이벤트
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// 현재 명령이 실행 가능한지 여부를 결정
        /// </summary>
        /// <param name="parameter">명령 파라미터</param>
        /// <returns>명령 실행 가능 여부</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// 명령 실행 메서드
        /// </summary>
        /// <param name="parameter">명령 파라미터</param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// 명령의 실행 가능 상태 갱신을 수동으로 트리거
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}