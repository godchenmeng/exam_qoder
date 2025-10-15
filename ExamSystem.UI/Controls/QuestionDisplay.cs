using System.Windows;
using System.Windows.Controls;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Enums;

namespace ExamSystem.UI.Controls
{
    /// <summary>
    /// 题目显示控件
    /// </summary>
    public class QuestionDisplay : Control
    {
        static QuestionDisplay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QuestionDisplay), 
                new FrameworkPropertyMetadata(typeof(QuestionDisplay)));
        }

        public static readonly DependencyProperty QuestionProperty =
            DependencyProperty.Register(nameof(Question), typeof(Question), typeof(QuestionDisplay),
                new PropertyMetadata(null, OnQuestionChanged));

        public Question Question
        {
            get => (Question)GetValue(QuestionProperty);
            set => SetValue(QuestionProperty, value);
        }

        public static readonly DependencyProperty ShowAnswerProperty =
            DependencyProperty.Register(nameof(ShowAnswer), typeof(bool), typeof(QuestionDisplay),
                new PropertyMetadata(false));

        public bool ShowAnswer
        {
            get => (bool)GetValue(ShowAnswerProperty);
            set => SetValue(ShowAnswerProperty, value);
        }

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(QuestionDisplay),
                new PropertyMetadata(false));

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public static readonly DependencyProperty SelectedAnswerProperty =
            DependencyProperty.Register(nameof(SelectedAnswer), typeof(string), typeof(QuestionDisplay),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string SelectedAnswer
        {
            get => (string)GetValue(SelectedAnswerProperty);
            set => SetValue(SelectedAnswerProperty, value);
        }

        private static void OnQuestionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 当题目改变时，可以执行一些初始化逻辑
        }
    }
}
