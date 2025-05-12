using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using HospitalManagementSystem.Helpers;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Services;

namespace HospitalManagementSystem.ViewModels
{
    /// <summary>
    /// 환자 관리 화면의 ViewModel
    /// </summary>
    public class PatientViewModel : ViewModelBase
    {
        private readonly PatientService _patientService;

        private ObservableCollection<Patient> _patients;
        private Patient _selectedPatient;
        private string _searchText;
        private bool _isEditing;
        private bool _isAdding;
        private Patient _editingPatient;

        /// <summary>
        /// 환자 목록
        /// </summary>
        public ObservableCollection<Patient> Patients
        {
            get => _patients;
            set => SetProperty(ref _patients, value);
        }

        /// <summary>
        /// 선택된 환자
        /// </summary>
        public Patient SelectedPatient
        {
            get => _selectedPatient;
            set
            {
                if (SetProperty(ref _selectedPatient, value))
                {
                    // 속성이 변경되면 Command 실행 가능 상태 갱신
                    EditPatientCommand.RaiseCanExecuteChanged();
                    DeletePatientCommand.RaiseCanExecuteChanged();

                    if (value != null)
                    {
                        // 선택된 환자 정보를 편집용 객체로 복사 (깊은 복사)
                        EditingPatient = new Patient
                        {
                            PatientId = value.PatientId,
                            FirstName = value.FirstName,
                            LastName = value.LastName,
                            DateOfBirth = value.DateOfBirth,
                            Gender = value.Gender,
                            PhoneNumber = value.PhoneNumber,
                            Email = value.Email,
                            Address = value.Address,
                            InsuranceNumber = value.InsuranceNumber,
                            Allergies = value.Allergies,
                            RegistrationDate = value.RegistrationDate
                        };
                    }
                }
            }
        }

        /// <summary>
        /// 검색어
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        /// <summary>
        /// 편집 모드 여부
        /// </summary>
        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        /// <summary>
        /// 추가 모드 여부
        /// </summary>
        public bool IsAdding
        {
            get => _isAdding;
            set => SetProperty(ref _isAdding, value);
        }

        /// <summary>
        /// 편집 중인 환자 정보
        /// </summary>
        public Patient EditingPatient
        {
            get => _editingPatient;
            set => SetProperty(ref _editingPatient, value);
        }

        /// <summary>
        /// 검색 명령
        /// </summary>
        public RelayCommand SearchCommand { get; }

        /// <summary>
        /// 추가 모드 시작 명령
        /// </summary>
        public RelayCommand AddNewCommand { get; }

        /// <summary>
        /// 환자 추가 명령
        /// </summary>
        public RelayCommand SaveNewCommand { get; }

        /// <summary>
        /// 추가 취소 명령
        /// </summary>
        public RelayCommand CancelAddCommand { get; }

        /// <summary>
        /// 편집 모드 시작 명령
        /// </summary>
        public RelayCommand EditPatientCommand { get; }

        /// <summary>
        /// 환자 정보 저장 명령
        /// </summary>
        public RelayCommand SaveEditCommand { get; }

        /// <summary>
        /// 편집 취소 명령
        /// </summary>
        public RelayCommand CancelEditCommand { get; }

        /// <summary>
        /// 환자 삭제 명령
        /// </summary>
        public RelayCommand DeletePatientCommand { get; }

        /// <summary>
        /// 생성자
        /// </summary>
        public PatientViewModel()
        {
            _patientService = new PatientService();

            // 환자 목록 초기화
            LoadPatients();

            // 새 환자 객체 초기화
            EditingPatient = new Patient();

            // 명령 초기화
            SearchCommand = new RelayCommand(ExecuteSearch);
            AddNewCommand = new RelayCommand(ExecuteAddNew);
            SaveNewCommand = new RelayCommand(ExecuteSaveNew);
            CancelAddCommand = new RelayCommand(ExecuteCancelAdd);
            EditPatientCommand = new RelayCommand(ExecuteEditPatient, CanEditPatient);
            SaveEditCommand = new RelayCommand(ExecuteSaveEdit);
            CancelEditCommand = new RelayCommand(ExecuteCancelEdit);
            DeletePatientCommand = new RelayCommand(ExecuteDeletePatient, CanDeletePatient);
        }

        /// <summary>
        /// 환자 목록 로드
        /// </summary>
        private void LoadPatients()
        {
            var patients = _patientService.GetAllPatients();
            Patients = new ObservableCollection<Patient>(patients);
        }

        /// <summary>
        /// 검색 실행
        /// </summary>
        private void ExecuteSearch(object parameter)
        {
            try
            {
                var patients = _patientService.SearchPatients(SearchText);
                Patients = new ObservableCollection<Patient>(patients);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"검색 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 신규 환자 추가 모드 시작
        /// </summary>
        private void ExecuteAddNew(object parameter)
        {
            IsAdding = true;
            EditingPatient = new Patient
            {
                DateOfBirth = DateTime.Now.AddYears(-30), // 기본값
                RegistrationDate = DateTime.Now
            };
        }

        /// <summary>
        /// 신규 환자 저장
        /// </summary>
        private void ExecuteSaveNew(object parameter)
        {
            try
            {
                _patientService.AddPatient(EditingPatient);

                // 리스트 새로고침
                LoadPatients();

                // 추가 모드 종료
                IsAdding = false;

                MessageBox.Show("환자가 성공적으로 추가되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"환자 추가 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 추가 취소
        /// </summary>
        private void ExecuteCancelAdd(object parameter)
        {
            IsAdding = false;
        }

        /// <summary>
        /// 환자 편집 모드 시작
        /// </summary>
        private void ExecuteEditPatient(object parameter)
        {
            if (SelectedPatient != null)
            {
                IsEditing = true;
            }
        }

        /// <summary>
        /// 환자 편집 가능 여부
        /// </summary>
        private bool CanEditPatient(object parameter)
        {
            return SelectedPatient != null;
        }

        /// <summary>
        /// 환자 정보 저장
        /// </summary>
        private void ExecuteSaveEdit(object parameter)
        {
            try
            {
                if (EditingPatient != null)
                {
                    _patientService.UpdatePatient(EditingPatient);

                    // 리스트 새로고침
                    LoadPatients();

                    // 편집 모드 종료
                    IsEditing = false;

                    MessageBox.Show("환자 정보가 성공적으로 수정되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"환자 정보 수정 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 편집 취소
        /// </summary>
        private void ExecuteCancelEdit(object parameter)
        {
            IsEditing = false;
        }

        /// <summary>
        /// 환자 삭제
        /// </summary>
        private void ExecuteDeletePatient(object parameter)
        {
            if (SelectedPatient != null)
            {
                var result = MessageBox.Show(
                    $"정말로 {SelectedPatient.FullName} 환자를 삭제하시겠습니까?",
                    "환자 삭제",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _patientService.DeletePatient(SelectedPatient.PatientId);

                        // 리스트 새로고침
                        LoadPatients();

                        MessageBox.Show("환자가 성공적으로 삭제되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"환자 삭제 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        /// <summary>
        /// 환자 삭제 가능 여부
        /// </summary>
        private bool CanDeletePatient(object parameter)
        {
            return SelectedPatient != null;
        }
    }
}