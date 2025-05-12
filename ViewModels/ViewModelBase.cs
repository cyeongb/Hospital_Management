using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HospitalManagementSystem.ViewModels
{
    /// <summary>
    /// 모든 ViewModel의 기본 클래스
    /// INotifyPropertyChanged 인터페이스를 구현하여 데이터 바인딩 지원
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 속성 값이 변경되었을 때 발생하는 이벤트
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 속성 변경 알림을 발생시키는 메서드
        /// </summary>
        /// <param name="propertyName">변경된 속성 이름 (자동으로 호출한 메서드 이름이 사용됨)</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 속성 값을 설정하고, 값이 변경되었을 경우 PropertyChanged 이벤트를 발생시키는 헬퍼 메서드
        /// </summary>
        /// <typeparam name="T">속성 타입</typeparam>
        /// <param name="storage">속성 값이 저장될 필드 참조</param>
        /// <param name="value">설정할 새 값</param>
        /// <param name="propertyName">속성 이름 (자동으로 호출한 메서드 이름이 사용됨)</param>
        /// <returns>값이 변경되었으면 true, 아니면 false</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}