using System;
using System.Linq;
using System.Security;

namespace Homework
{
	class VaccinationRequest
	{
		private int _ID;
		private string _patientName;
		private string _patientPhone;
		private string _vaccine;
		private DateOnly _date;
		private TimeOnly _startTime;
		private TimeOnly _endTime;

		private static TimeOnly _canStart = new TimeOnly(7, 0);
		private static TimeOnly _canEnd = new TimeOnly(21, 0);

		private static string[] _vaccineChoices
			= {"pfizer", "moderna", "astrazeneca", "coronavac"};

		public VaccinationRequest()
		{
			this.ID = 1;
			this.PatientName = "John Doe";
			this.PatientPhone = "+380123456789";
			this.Vaccine = "pfizer";
			this.Date = DateOnly.FromDateTime(DateTime.Today).AddDays(1);
			this.StartTime = VaccinationRequest._canStart;
			this.EndTime = VaccinationRequest._canEnd.AddHours(1);
			return;
		}

		public VaccinationRequest(
			int ID,
			string patientName,
			string patientPhone,
			string vaccine,
			DateOnly date,
			TimeOnly startTime,
			TimeOnly endTime
		)
		{
			this.ID = ID;
			this.PatientName = patientName;
			this.PatientPhone = patientPhone;
			this.Vaccine = vaccine;
			this.Date = date;
			this.StartTime = startTime;
			this.EndTime = endTime;
			return;
		}

		public VaccinationRequest(
			string ID,
			string patientName,
			string patientPhone,
			string vaccine,
			DateOnly date,
			TimeOnly startTime,
			TimeOnly endTime
		)
		{
			this.ID = int.Parse(ID);
			this.PatientName = patientName;
			this.PatientPhone = patientPhone;
			this.Vaccine = vaccine;
			this.Date = date;
			this.StartTime = startTime;
			this.EndTime = endTime;
			return;
		}

		public int ID
		{
			get => this._ID;
			set => this._ID = IntValidators.GreaterThanZero(value.ToString())
			                  ?? throw new ArgumentException("ID cannot be lesser than 0.");
		}

		public string PatientName
		{
			get => this._patientName;
			set => this._patientName = value;
		}

		public string PatientPhone
		{
			get => this._patientPhone;
			set => this._patientPhone = StringValidators.IsUaPhoneNumber(value)
			                            ?? throw new ArgumentException("Not a phone number was passed.");
		}

		public string Vaccine
		{
			get => this._vaccine;
			set => this._vaccine = value;
		}

		public DateOnly Date
		{
			get { return (this._date); }
			set
			{
				if (value < DateOnly.FromDateTime(DateTime.Today))
				{
					throw new Exception("Date cannot be lesser than today.");
				}

				this._date = value;
				return;
			}
		}

		public TimeOnly StartTime
		{
			get { return (this._startTime); }
			set
			{
				if ((this.EndTime != default) && (value > this.EndTime))
				{
					throw new ArgumentException(
						"Start time cannot be greater than end time");
				}
				

				this._startTime = value;
				return;
			}
		}

		public TimeOnly EndTime
		{
			get { return (this._endTime); }
			set
			{
				if (value <= this.StartTime)
				{
					throw new ArgumentException(
						"End time cannot be lesser than start time.");
				}


				this._endTime = value;
				return;
			}
		}

		public override string ToString()
		{
			string res = "";

			var elem_arr = this.GetType().GetProperties().ToArray();
			res += elem_arr[0].GetValue(this)?.ToString();
			for (int i = 1; i < elem_arr.Count(); ++i)
			{
				res += (' ' + elem_arr[i].GetValue(this)?.ToString());
			}

			return res;
		}

		public static bool TryStartTime(string? str)
			=> (TimeValidators.TryTime(str,
				VaccinationRequest._canStart,
				VaccinationRequest._canEnd));

		public static bool TryEndTime(string? str)
			=> (TimeValidators.TryTime(str,
				VaccinationRequest._canStart.AddHours(1),
				VaccinationRequest._canEnd.AddHours(1)));

		public static bool TryVaccine(string? str)
			=> ((str != null)
			    && (_vaccineChoices.Contains(str.ToLower()) == true));

		public static string? IsVaccine(string? str)
			=> ((VaccinationRequest.TryVaccine(str) == true) ? str : null);
	}
	
}