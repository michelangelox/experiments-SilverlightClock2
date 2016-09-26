using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Windows.Media.Imaging;

namespace Silverlight_Clock_2
{
	public partial class Page : UserControl
	{
		private Storyboard StoryBoard;

		private int fontSize = 22;

		private List<string> weekDayNames = new List<string>();
		private List<string> monthNames = new List<string>();

		private Line timeLine = new Line();

		public Page()
		{
			InitializeComponent();
			InitializeClock();
			StartClock();
		}

		private void InitializeClock()
		{
			// Add the timeline----------------------------------------------------
			this.timeLine.Stroke = new SolidColorBrush(Colors.Red);
			this.timeLine.X1 = LayoutRoot.Width / 2;
			this.timeLine.Y1 = 0;
			this.timeLine.X2 = LayoutRoot.Width / 2;
			this.timeLine.Y2 = LayoutRoot.Height - currentTime.Height + 5;
			this.timeLine.SetValue(Canvas.ZIndexProperty, 1000);

			LayoutRoot.Children.Add(this.timeLine);
			//---------------------------------------------------------------------

			//populate the weekday name list
			weekDayNames.Add("Sunday");
			weekDayNames.Add("Monday");
			weekDayNames.Add("Tuesday");
			weekDayNames.Add("Wednesday");
			weekDayNames.Add("Thursday");
			weekDayNames.Add("Friday");
			weekDayNames.Add("Saturday");

			//populate the monthday list
			monthNames.Add("December");
			monthNames.Add("January");
			monthNames.Add("February");
			monthNames.Add("March");
			monthNames.Add("April");
			monthNames.Add("May");
			monthNames.Add("June");
			monthNames.Add("July");
			monthNames.Add("Augustus");
			monthNames.Add("September");
			monthNames.Add("October");
			monthNames.Add("November");

			generateBasicCanvas(canvasSeconds, 60, "seconds", 40);
			generateBasicCanvas(canvasMinutes, 60, "minutes", 40);
			generateBasicCanvas(canvasHours, 24, "hours", 60);
			generateBasicCanvas(canvasDayNames, 7, "daynames", 150);
			generateBasicCanvas(canvasDays, 31, "days", 60);
			generateBasicCanvas(canvasMonths, 12, "months", 150);
			generateBasicCanvas(canvasYears, 20, "years", 80);
		}

		private void generateBasicCanvas(Canvas _canvas, int _units, string _type, int _width)
		{
			_canvas.Height = 40;
			_canvas.Width = LayoutRoot.Width;

			_canvas.Margin = new Thickness(1, 1, 1, 1);

			for (int i = 1; i <= _units; i++)
			{
				//create the individual textboxes
				TextBox digit = new TextBox();
				digit.Width = _width;
				digit.Height = this.canvasSeconds.Height;
				digit.BorderThickness = new Thickness(2);
				digit.BorderBrush = new SolidColorBrush(Colors.Black);
				digit.Background = new SolidColorBrush(Colors.LightGray);
				digit.FontSize = this.fontSize;
				digit.TextAlignment = TextAlignment.Center;

				//orders the boxes 
				digit.SetValue(Canvas.LeftProperty, (digit.Width) * i);

				digit.IsReadOnly = true;
				digit.IsTabStop = false;


				ImageBrush digitBackground = new ImageBrush();
				digitBackground.ImageSource = new BitmapImage(new Uri("background.jpg", UriKind.Relative));
				digit.Background = digitBackground;

				//sets the content of the textboxes
				switch (_type)
				{
					case "daynames":
						digit.Text = weekDayNames[i - 1];
						break;

					case "months":
						digit.Text = monthNames[i - 1];
						break;

					case "years":
						digit.Text = (2000 + i).ToString();
						break;

					default:
						digit.Text = (i > 9) ? i.ToString() : "0" + i.ToString();
						break;
				}

				//determines the clipping area
				RectangleGeometry clipArea = new RectangleGeometry();
				clipArea.Rect = new Rect(0, 0, _canvas.Width, _canvas.Height);
				_canvas.Clip = clipArea;

				_canvas.Children.Add(digit);
			}
		}

		private void StartClock()
		{
			this.StoryBoard = new Storyboard();
			this.StoryBoard.Duration = TimeSpan.FromMilliseconds(1);
			this.StoryBoard.Completed += new EventHandler(StoryBoardSeconds_Completed);

			StoryBoard.Begin();
		}

		private void StoryBoardSeconds_Completed(object sender, EventArgs e)
		{
			currentTime.Text = DateTime.Now.TimeOfDay + ", " + DateTime.Now.ToLongDateString() + ".";

			MoveGears(canvasSeconds,	"seconds",		DateTime.Now.Second);
			MoveGears(canvasMinutes,	"minutes",		DateTime.Now.Minute);
			MoveGears(canvasHours,		"hours",		DateTime.Now.Hour);
			MoveGears(canvasDayNames,	"dayNames",		DateTime.Now.Day);
			MoveGears(canvasDays,		"days",			DateTime.Now.Day);
			MoveGears(canvasMonths,		"monthNames",	DateTime.Now.Month);
			MoveGears(canvasYears,		"years",		DateTime.Now.Year);

			StoryBoard.Begin();
		}

		private void MoveGears(Canvas _canvas, string _type, int _timeMargin)
		{
			DateTime _now = DateTime.Now;

			double _moveUnit = 0;
			int _digitValue = 0;

			foreach (TextBox digit in _canvas.Children)
			{
				switch (_type)
				{
					case "dayNames":
						_digitValue = weekDayNames.IndexOf(digit.Text);
						break;

					case "monthNames":
						_digitValue = monthNames.IndexOf(digit.Text);
						break;

					default:
						_digitValue = Convert.ToInt32(digit.Text);
						break;
				}

				int _unitMargin = Convert.ToInt32(this.LayoutRoot.Width / digit.Width);

				if (_type != "dayNames")
				{
					//used to tie the wheel around the initial digits...
					if (_timeMargin > _canvas.Children.Count - _unitMargin && _digitValue < _unitMargin)
					{
						_digitValue += _canvas.Children.Count;
					}

					//used to tie the wheel around the last digits...
					if (_timeMargin < _unitMargin && _digitValue > _canvas.Children.Count - _unitMargin)
					{
						_digitValue -= _canvas.Children.Count;
					}
				}

				//interchanges the named objects
				switch (_type)
				{
					case "seconds":

						_moveUnit = this.timeLine.X1 + ((_digitValue - _now.Second) * digit.Width) - ((double)(_now.Millisecond) * (digit.Width / 1000));
						break;
					case "minutes":

						_moveUnit = this.timeLine.X1 + ((_digitValue - _now.Minute) * digit.Width) - ((double)(_now.Second + (_now.Millisecond * 0.001)) * (digit.Width / 60));
						break;

					case "hours":
						_moveUnit = this.timeLine.X1 + ((_digitValue - _now.Hour) * digit.Width) - ((double)(_now.Minute + (_now.Second * 0.016)) * (digit.Width / 60));
						break;

					case "dayNames":
						_moveUnit = this.timeLine.X1 + ((_digitValue - (int)_now.DayOfWeek) * digit.Width) - ((_now.Hour) * (digit.Width / 24));
						break;

					case "days":
						_moveUnit = this.timeLine.X1 + ((_digitValue - _now.Day) * digit.Width) - ((_now.Hour) * (digit.Width / 24));
						break;

					case "monthNames":
						_moveUnit = this.timeLine.X1 + ((_digitValue - _now.Month) * digit.Width) - ((_now.Day) * (digit.Width / 30));
						break;

					case "years":
						_moveUnit = this.timeLine.X1 + ((_digitValue - _now.Year) * digit.Width) - ((_now.Month) * (digit.Width / 12));
						break;

					default:
						break;
				}
				digit.SetValue(Canvas.LeftProperty, _moveUnit);
			}
		}
	}
}