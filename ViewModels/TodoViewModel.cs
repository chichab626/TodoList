using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TodoList.Data;
using TodoList.ViewModels;

namespace TodoList
{
    public class TodoViewModel : BaseViewModel
    {
        private bool _isCompleted;
        private Category _category;
        private DateTime _dueDate = DateTime.Now;
        private TimeSpan _dueTime = DateTime.Now.TimeOfDay;

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public bool IsCompleted
        {
            get => _isCompleted;
            set => SetProperty(ref _isCompleted, value);
        }

        public Category Category
        {
            get => _category;
            set
            {
                if (SetProperty(ref _category, value))
                {
                    OnPropertyChanged(nameof(CategoryName));
                    OnPropertyChanged(nameof(CategoryColor));
                }
            }
        }

        public string CategoryName => Category?.Name ?? "General";
        public string CategoryColor => Category?.Color ?? "#808080";

        public DateTime CreatedAt { get; set; }

        public DateTime DueDate
        {
            get => _dueDate;
            set => SetProperty(ref _dueDate, value);
        }

        public TimeSpan DueTime
        {
            get => _dueTime;
            set => SetProperty(ref _dueTime, value);
        }

        public bool IsDueExpired
        {
            get
            {
                DateTime dueDateTime = DueDate.Date.Add(DueTime);
                return dueDateTime < DateTime.Now;
            }
        }
    }
}
