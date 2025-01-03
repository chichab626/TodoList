﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TodoList.Data;

namespace TodoList
{
    public class TodoViewModel : INotifyPropertyChanged
    {
        private bool _isCompleted;
        private Category _category;
        private DateTime _dueDate = DateTime.Now;
        private TimeSpan _dueTime = DateTime.Now.TimeOfDay;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    OnPropertyChanged();
                }
            }
        }

        public Category Category
        {
            get => _category;
            set
            {
                if (_category != value)
                {
                    _category = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CategoryName));
                    OnPropertyChanged(nameof(CategoryColor));
                }
            }
        }

        public string CategoryName => Category?.Name ?? "General";
        public string CategoryColor => Category?.Color ?? "#808080";

        public DateTime CreatedAt { get; set; }

        // New properties for Due Date and Due Time
        public DateTime DueDate
        {
            get => _dueDate;
            set
            {
                _dueDate = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan DueTime
        {
            get => _dueTime;
            set
            {
                _dueTime = value;
                OnPropertyChanged();
            }
        }

        public bool IsDueExpired
        {
            get
            {
                DateTime dueDateTime = DueDate.Date.Add(DueTime);
                return dueDateTime < DateTime.Now;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
