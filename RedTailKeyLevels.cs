//
// Copyright (C) 2018, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//
#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Core;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
using SharpDX.DirectWrite;

#endregion

//This namespace holds Indicators in this folder and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Indicators
{
	[TypeConverter("NinjaTrader.NinjaScript.Indicators.RedTailKeyLevelsTypeConverter")]
	public class RedTailKeyLevels : Indicator
	{
		private DateTime				cacheMonthlyEndDate		= Globals.MinDate;
		private DateTime				cacheSessionDate		= Globals.MinDate;
		private DateTime				cacheSessionEnd			= Globals.MinDate;
		private DateTime				cacheTime;
		private DateTime				cacheWeeklyEndDate		= Globals.MinDate;
		private DateTime				currentDate				= Globals.MinDate;
		private DateTime				currentMonth			= Globals.MinDate;
		private DateTime				currentWeek				= Globals.MinDate;
		private DateTime				sessionDateTmp			= Globals.MinDate;
		private RedTailHLCMode		priorDayHlc;
		private RedTailPivotRange				pivotRangeType;
		private SessionIterator			storedSession;
		private double					currentClose;
		private double					currentHigh				= double.MinValue;
		private double					currentLow				= double.MaxValue;
		private double					dailyBarClose			= double.MinValue;
		private double					dailyBarHigh			= double.MinValue;
		private double					dailyBarLow				= double.MinValue;
		private double					pp;
		private double					r1;
		private double					r2;
		private double					r3;
		private double					s1;
		private double					s2;
		private double					s3;
		private double					r1m;
		private double					r2m;
		private double					r3m;
		private double					s1m;
		private double					s2m;
		private double					s3m;
		private double					ph;
		private double					pl;
		private double					pwh;
		private double					pwl;
		private double					pmh;
		private double					pml;
		private double					mondayHigh;
		private double					mondayLow;
		private double					userDefinedClose;
		private double					userDefinedHigh;
		private double					userDefinedLow;
		private int						cacheBar;
		private int						width					= 250;
		private double					mergeTolerance			= 0.0;
		private bool					showMidlines			= true;
		private bool					showPivots				= true;
		private bool					showPreviousDay			= true;
		private bool					showPreviousWeek		= true;
		private bool					showPreviousMonth		= true;
		private bool					showMondayRange			= true;
		private bool					showGlobexSession		= true;
		private bool					showRTHSession			= true;
		private bool					showFibLevels			= true;
		private double					fib1					= 0.236;
		private double					fib2					= 0.382;
		private double					fib3					= 0.500;
		private double					fib4					= 0.618;
		private double					fib5					= 0.786;
		private double					fib6					= 0.0;
		private double					fib7					= 0.0;
		private double					fib8					= 0.0;
		private double					fib9					= 0.0;
		private double					fib10					= 0.0;
		private readonly List<int>		newSessionBarIdxArr		= new List<int>();
		private DateTime				currentWeekHigh			= Globals.MinDate;
		private DateTime				currentWeekLow			= Globals.MinDate;
		private double					weeklyHigh				= double.MinValue;
		private double					weeklyLow				= double.MaxValue;
		private DateTime				currentMonthHigh		= Globals.MinDate;
		private DateTime				currentMonthLow			= Globals.MinDate;
		private double					monthlyHigh				= double.MinValue;
		private double					monthlyLow				= double.MaxValue;
		private DateTime				currentMondayWeek		= Globals.MinDate;
		private double					currentMondayHigh		= double.MinValue;
		private double					currentMondayLow		= double.MaxValue;
		private DateTime				currentGlobexWeek		= Globals.MinDate;
		private double					globexHigh				= double.MinValue;
		private double					globexLow				= double.MaxValue;
		private DateTime				currentRTHDay			= Globals.MinDate;
		private double					currentRTHHigh			= double.MinValue;
		private double					currentRTHLow			= double.MaxValue;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Name					= "RedTail Key Levels v1.1";
				Calculate				= Calculate.OnBarClose;
				DisplayInDataBox		= true;
				DrawOnPricePanel		= false;
				IsAutoScale				= false;
				IsOverlay				= true;
				PaintPriceMarkers		= true;
				ScaleJustification		= ScaleJustification.Right;
				
				ShowHistorical 			= false;
				ShowMidlines			= true;
				ShowPivots				= true;
				ShowPreviousDay			= true;
				ShowPreviousWeek		= true;
				ShowPreviousMonth		= true;
				ShowMondayRange			= true;
				ShowGlobexSession		= true;
				ShowRTHSession			= true;
				ShowFibLevels			= true;
				Fib1					= 0.236;
				Fib2					= 0.382;
				Fib3					= 0.500;
				Fib4					= 0.618;
				Fib5					= 0.786;
				Fib6					= 0.0;
				Fib7					= 0.0;
				Fib8					= 0.0;
				Fib9					= 0.0;
				Fib10					= 0.0;
				MergeTolerance			= 0.0;
				
				AddPlot(new Stroke(Brushes.Yellow, 		DashStyleHelper.Dot, 1), 	PlotStyle.Line, "PP");
				AddPlot(new Stroke(Brushes.SkyBlue, 	DashStyleHelper.Dot, 1),	PlotStyle.Line, "R1");
				AddPlot(new Stroke(Brushes.Aqua, 		DashStyleHelper.Dot, 1),	PlotStyle.Line, "R2");
				AddPlot(new Stroke(Brushes.LightBlue, 	DashStyleHelper.Dot, 1),	PlotStyle.Line, "R3");
				AddPlot(new Stroke(Brushes.Red, 		DashStyleHelper.Dot, 1),	PlotStyle.Line, "S1");
				AddPlot(new Stroke(Brushes.Coral, 		DashStyleHelper.Dot, 1),	PlotStyle.Line, "S2");
				AddPlot(new Stroke(Brushes.Pink, 		DashStyleHelper.Dot, 1),	PlotStyle.Line, "S3");
				AddPlot(new Stroke(Brushes.LightGray, 	DashStyleHelper.Dot, 1),	PlotStyle.Line, "R1M");
				AddPlot(new Stroke(Brushes.LightGray, 	DashStyleHelper.Dot, 1),	PlotStyle.Line, "R2M");
				AddPlot(new Stroke(Brushes.LightGray, 	DashStyleHelper.Dot, 1),	PlotStyle.Line, "R3M");
				AddPlot(new Stroke(Brushes.LightGray, 	DashStyleHelper.Dot, 1),	PlotStyle.Line, "S1M");
				AddPlot(new Stroke(Brushes.LightGray, 	DashStyleHelper.Dot, 1),	PlotStyle.Line, "S2M");
				AddPlot(new Stroke(Brushes.LightGray, 	DashStyleHelper.Dot, 1),	PlotStyle.Line, "S3M");
				AddPlot(new Stroke(Brushes.Lime, 		DashStyleHelper.Dot, 1),	PlotStyle.Line, "PDH");
				AddPlot(new Stroke(Brushes.Magenta, 	DashStyleHelper.Dot, 1),	PlotStyle.Line, "PDL");
				AddPlot(new Stroke(Brushes.LimeGreen, 	DashStyleHelper.Dot, 1),	PlotStyle.Line, "PWH");
				AddPlot(new Stroke(Brushes.Orchid, 		DashStyleHelper.Dot, 1),	PlotStyle.Line, "PWL");
				AddPlot(new Stroke(Brushes.Cyan, 		DashStyleHelper.Dot, 1),	PlotStyle.Line, "PMH");
				AddPlot(new Stroke(Brushes.DeepPink, 	DashStyleHelper.Dot, 1),	PlotStyle.Line, "PML");
				AddPlot(new Stroke(Brushes.Gold, 		DashStyleHelper.Dot, 1),	PlotStyle.Line, "MH");
				AddPlot(new Stroke(Brushes.Orange, 		DashStyleHelper.Dot, 1),	PlotStyle.Line, "ML");
				AddPlot(new Stroke(Brushes.DodgerBlue, 	DashStyleHelper.Dot, 1),	PlotStyle.Line, "GH");
				AddPlot(new Stroke(Brushes.Tomato, 		DashStyleHelper.Dot, 1),	PlotStyle.Line, "GL");
				AddPlot(new Stroke(Brushes.MediumSpringGreen, DashStyleHelper.Dot, 1), PlotStyle.Line, "NYH");
				AddPlot(new Stroke(Brushes.HotPink, 	DashStyleHelper.Dot, 1),	PlotStyle.Line, "NYL");
				AddPlot(new Stroke(Brushes.Violet, 		DashStyleHelper.Dot, 1),	PlotStyle.Line, "Fib1");
				AddPlot(new Stroke(Brushes.Violet, 		DashStyleHelper.Dot, 1),	PlotStyle.Line, "Fib2");
				AddPlot(new Stroke(Brushes.Violet, 		DashStyleHelper.Dot, 1),	PlotStyle.Line, "Fib3");
				AddPlot(new Stroke(Brushes.Violet, 		DashStyleHelper.Dot, 1),	PlotStyle.Line, "Fib4");
				AddPlot(new Stroke(Brushes.Violet, 		DashStyleHelper.Dot, 1),	PlotStyle.Line, "Fib5");
				AddPlot(new Stroke(Brushes.Violet, 		DashStyleHelper.Dot, 1),	PlotStyle.Line, "Fib6");
				AddPlot(new Stroke(Brushes.Violet, 		DashStyleHelper.Dot, 1),	PlotStyle.Line, "Fib7");
				AddPlot(new Stroke(Brushes.Violet, 		DashStyleHelper.Dot, 1),	PlotStyle.Line, "Fib8");
				AddPlot(new Stroke(Brushes.Violet, 		DashStyleHelper.Dot, 1),	PlotStyle.Line, "Fib9");
				AddPlot(new Stroke(Brushes.Violet, 		DashStyleHelper.Dot, 1),	PlotStyle.Line, "Fib10");
			}
			else if (State == State.Configure)
			{
				if (priorDayHlc == RedTailHLCMode.DailyBars)
					AddDataSeries(BarsPeriodType.Day, 1);
			}
			else if (State == State.DataLoaded)
			{
				storedSession = new SessionIterator(Bars);
			}
			else if (State == State.Historical)
			{
				if (priorDayHlc == RedTailHLCMode.DailyBars && BarsArray[1].DayCount <= 0)
				{
					Draw.TextFixed(this, "NinjaScriptInfo", NinjaTrader.Custom.Resource.PiviotsDailyDataError, TextPosition.BottomRight);
					Log(NinjaTrader.Custom.Resource.PiviotsDailyDataError, LogLevel.Error);
					return;
				}

				if (!Bars.BarsType.IsIntraday && BarsPeriod.BarsPeriodType != BarsPeriodType.Day && (BarsPeriod.BarsPeriodType != BarsPeriodType.HeikenAshi && BarsPeriod.BarsPeriodType != BarsPeriodType.Volumetric || BarsPeriod.BaseBarsPeriodType != BarsPeriodType.Day))
				{
					Draw.TextFixed(this, "NinjaScriptInfo", NinjaTrader.Custom.Resource.PiviotsDailyBarsError, TextPosition.BottomRight);
					Log(NinjaTrader.Custom.Resource.PiviotsDailyBarsError, LogLevel.Error);
				}
				if ((BarsPeriod.BarsPeriodType == BarsPeriodType.Day || ((BarsPeriod.BarsPeriodType == BarsPeriodType.HeikenAshi || BarsPeriod.BarsPeriodType == BarsPeriodType.Volumetric) && BarsPeriod.BaseBarsPeriodType == BarsPeriodType.Day)) && pivotRangeType == RedTailPivotRange.Daily)
				{
					Draw.TextFixed(this, "NinjaScriptInfo", NinjaTrader.Custom.Resource.PiviotsWeeklyBarsError, TextPosition.BottomRight);
					Log(NinjaTrader.Custom.Resource.PiviotsWeeklyBarsError, LogLevel.Error);
				}
				if ((BarsPeriod.BarsPeriodType == BarsPeriodType.Day || ((BarsPeriod.BarsPeriodType == BarsPeriodType.HeikenAshi || BarsPeriod.BarsPeriodType == BarsPeriodType.Volumetric) && BarsPeriod.BaseBarsPeriodType == BarsPeriodType.Day)) && BarsPeriod.Value > 1)
				{
					Draw.TextFixed(this, "NinjaScriptInfo", NinjaTrader.Custom.Resource.PiviotsPeriodTypeError, TextPosition.BottomRight);
					Log(NinjaTrader.Custom.Resource.PiviotsPeriodTypeError, LogLevel.Error);
				}
				if ((priorDayHlc == RedTailHLCMode.DailyBars &&
					(pivotRangeType == RedTailPivotRange.Monthly && BarsArray[1].GetTime(0).Date >= BarsArray[1].GetTime(BarsArray[1].Count - 1).Date.AddMonths(-1)
					|| pivotRangeType == RedTailPivotRange.Weekly && BarsArray[1].GetTime(0).Date >= BarsArray[1].GetTime(BarsArray[1].Count - 1).Date.AddDays(-7)
					|| pivotRangeType == RedTailPivotRange.Daily && BarsArray[1].GetTime(0).Date >= BarsArray[1].GetTime(BarsArray[1].Count - 1).Date.AddDays(-1)))
					|| pivotRangeType == RedTailPivotRange.Monthly && BarsArray[0].GetTime(0).Date >= BarsArray[0].GetTime(BarsArray[0].Count - 1).Date.AddMonths(-1)
					|| pivotRangeType == RedTailPivotRange.Weekly && BarsArray[0].GetTime(0).Date >= BarsArray[0].GetTime(BarsArray[0].Count - 1).Date.AddDays(-7)
					|| pivotRangeType == RedTailPivotRange.Daily && BarsArray[0].GetTime(0).Date >= BarsArray[0].GetTime(BarsArray[0].Count - 1).Date.AddDays(-1)
					)
				{
					Draw.TextFixed(this, "NinjaScriptInfo", NinjaTrader.Custom.Resource.PiviotsInsufficentDataError, TextPosition.BottomRight);
					Log(NinjaTrader.Custom.Resource.PiviotsInsufficentDataError, LogLevel.Error);
				}
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0)
				return;

			if ((priorDayHlc == RedTailHLCMode.DailyBars && BarsArray[1].DayCount <= 0)
				|| (!Bars.BarsType.IsIntraday && BarsPeriod.BarsPeriodType != BarsPeriodType.Day && (BarsPeriod.BarsPeriodType != BarsPeriodType.HeikenAshi && BarsPeriod.BarsPeriodType != BarsPeriodType.Volumetric || BarsPeriod.BaseBarsPeriodType != BarsPeriodType.Day))
				|| ((BarsPeriod.BarsPeriodType == BarsPeriodType.Day || ((BarsPeriod.BarsPeriodType == BarsPeriodType.HeikenAshi || BarsPeriod.BarsPeriodType == BarsPeriodType.Volumetric) && BarsPeriod.BaseBarsPeriodType == BarsPeriodType.Day)) && pivotRangeType == RedTailPivotRange.Daily)
				|| ((BarsPeriod.BarsPeriodType == BarsPeriodType.Day || ((BarsPeriod.BarsPeriodType == BarsPeriodType.HeikenAshi || BarsPeriod.BarsPeriodType == BarsPeriodType.Volumetric) && BarsPeriod.BaseBarsPeriodType == BarsPeriodType.Day)) && BarsPeriod.Value > 1)
				|| ((priorDayHlc == RedTailHLCMode.DailyBars && (pivotRangeType == RedTailPivotRange.Monthly && BarsArray[1].GetTime(0).Date >= BarsArray[1].GetTime(BarsArray[1].Count - 1).Date.AddMonths(-1)
				|| pivotRangeType == RedTailPivotRange.Weekly && BarsArray[1].GetTime(0).Date >= BarsArray[1].GetTime(BarsArray[1].Count - 1).Date.AddDays(-7)
				|| pivotRangeType == RedTailPivotRange.Daily && BarsArray[1].GetTime(0).Date >= BarsArray[1].GetTime(BarsArray[1].Count - 1).Date.AddDays(-1)))
				|| pivotRangeType == RedTailPivotRange.Monthly && BarsArray[0].GetTime(0).Date >= BarsArray[0].GetTime(BarsArray[0].Count - 1).Date.AddMonths(-1)
				|| pivotRangeType == RedTailPivotRange.Weekly && BarsArray[0].GetTime(0).Date >= BarsArray[0].GetTime(BarsArray[0].Count - 1).Date.AddDays(-7)
				|| pivotRangeType == RedTailPivotRange.Daily && BarsArray[0].GetTime(0).Date >= BarsArray[0].GetTime(BarsArray[0].Count - 1).Date.AddDays(-1)))
				return;

			RemoveDrawObject("NinjaScriptInfo");

			if (PriorDayHlc == RedTailHLCMode.DailyBars && CurrentBars[1] >= 0)
			{
				// Get daily bars like this to avoid situation where primary series moves to next session before previous day OHLC are added
				if (cacheTime != Times[0][0])
				{
					cacheTime	= Times[0][0];
					cacheBar	= BarsArray[1].GetBar(Times[0][0]);
				}
				dailyBarHigh	= BarsArray[1].GetHigh(cacheBar);
				dailyBarLow		= BarsArray[1].GetLow(cacheBar);
				dailyBarClose	= BarsArray[1].GetClose(cacheBar);
			}
			else
			{
				dailyBarHigh	= double.MinValue;
				dailyBarLow		= double.MinValue;
				dailyBarClose	= double.MinValue;
			}

			double high		= (dailyBarHigh == double.MinValue)		? Highs[0][0]	: dailyBarHigh;
			double low		= (dailyBarLow == double.MinValue)		? Lows[0][0]	: dailyBarLow;
			double close	= (dailyBarClose == double.MinValue)	? Closes[0][0]	: dailyBarClose;

			DateTime lastBarTimeStamp = GetLastBarSessionDate(Times[0][0], pivotRangeType);

			if ((currentDate != Globals.MinDate && pivotRangeType == RedTailPivotRange.Daily && lastBarTimeStamp != currentDate)
				|| (currentWeek != Globals.MinDate && pivotRangeType == RedTailPivotRange.Weekly && lastBarTimeStamp != currentWeek)
				|| (currentMonth != Globals.MinDate && pivotRangeType == RedTailPivotRange.Monthly && lastBarTimeStamp != currentMonth))
			{
				pp				= (currentHigh + currentLow + currentClose) / 3;
				s1				= 2 * pp - currentHigh;
				r1				= 2 * pp - currentLow;
				s2				= pp - (currentHigh - currentLow);
				r2				= pp + (currentHigh - currentLow);
				s3				= pp - 2 * (currentHigh - currentLow);
				r3				= pp + 2 * (currentHigh - currentLow);
				pl				= currentLow;
				s1m				= (pp - s1) / 2 + s1;
				r1m				= (r1 - pp) / 2 + pp;
				s2m				= (s1 - s2) / 2 + s2;
				r2m				= (r2 - r1) / 2 + r1;
				s3m				= (s2 - s3) / 2 + s3;
				r3m				= (r3 - r2) / 2 + r2;
				ph				= currentHigh;
				currentClose	= (priorDayHlc == RedTailHLCMode.UserDefinedValues) ? UserDefinedClose	: close;
				currentHigh		= (priorDayHlc == RedTailHLCMode.UserDefinedValues) ? UserDefinedHigh	: high;
				currentLow		= (priorDayHlc == RedTailHLCMode.UserDefinedValues) ? UserDefinedLow	: low;
			}
			else
			{
				currentClose	= (priorDayHlc == RedTailHLCMode.UserDefinedValues) ? UserDefinedClose	: close;
				currentHigh		= (priorDayHlc == RedTailHLCMode.UserDefinedValues) ? UserDefinedHigh	: Math.Max(currentHigh, high);
				currentLow		= (priorDayHlc == RedTailHLCMode.UserDefinedValues) ? UserDefinedLow	: Math.Min(currentLow, low);
			}


			if (pivotRangeType == RedTailPivotRange.Daily)
				currentDate = lastBarTimeStamp;
			if (pivotRangeType == RedTailPivotRange.Weekly)
				currentWeek = lastBarTimeStamp;
			if (pivotRangeType == RedTailPivotRange.Monthly)
				currentMonth = lastBarTimeStamp;

			// Track weekly high/low for PWH/PWL calculation
			// Get the current week's end date (Friday 5pm)
			DateTime currentWeekEnd = GetWeekEndDate(Times[0][0]);
			
			if (currentWeekHigh == Globals.MinDate)
			{
				currentWeekHigh = currentWeekEnd;
				currentWeekLow = currentWeekEnd;
			}
			
			// Check if we've moved to a new week
			if (currentWeekEnd != currentWeekHigh)
			{
				// Store the previous week's high/low
				pwh = weeklyHigh;
				pwl = weeklyLow;
				
				// Reset for new week
				weeklyHigh = high;
				weeklyLow = low;
				currentWeekHigh = currentWeekEnd;
				currentWeekLow = currentWeekEnd;
			}
			else
			{
				// Update current week's high/low
				weeklyHigh = Math.Max(weeklyHigh, high);
				weeklyLow = Math.Min(weeklyLow, low);
			}

			// Track monthly high/low for PMH/PML calculation
			// Get the current month's end date (last day of month at 5pm)
			DateTime currentMonthEnd = GetMonthEndDate(Times[0][0]);
			
			if (currentMonthHigh == Globals.MinDate)
			{
				currentMonthHigh = currentMonthEnd;
				currentMonthLow = currentMonthEnd;
			}
			
			// Check if we've moved to a new month
			if (currentMonthEnd != currentMonthHigh)
			{
				// Store the previous month's high/low
				pmh = monthlyHigh;
				pml = monthlyLow;
				
				// Reset for new month
				monthlyHigh = high;
				monthlyLow = low;
				currentMonthHigh = currentMonthEnd;
				currentMonthLow = currentMonthEnd;
			}
			else
			{
				// Update current month's high/low
				monthlyHigh = Math.Max(monthlyHigh, high);
				monthlyLow = Math.Min(monthlyLow, low);
			}

			// Track Monday's range (Sunday 6pm to Monday 5pm)
			DateTime mondayWeekEnd = GetMondayWeekEnd(Times[0][0]);
			
			if (currentMondayWeek == Globals.MinDate)
			{
				currentMondayWeek = mondayWeekEnd;
			}
			
			// Check if we've moved to a new week (Tuesday started)
			if (mondayWeekEnd != currentMondayWeek)
			{
				// Store Monday's range
				mondayHigh = currentMondayHigh;
				mondayLow = currentMondayLow;
				
				// Reset for new week's Monday
				currentMondayHigh = high;
				currentMondayLow = low;
				currentMondayWeek = mondayWeekEnd;
			}
			else
			{
				// Update current Monday's high/low
				currentMondayHigh = Math.Max(currentMondayHigh, high);
				currentMondayLow = Math.Min(currentMondayLow, low);
			}

			// Track Globex session (Sunday 6pm to Friday 5pm)
			DateTime globexWeekEnd = GetWeekEndDate(Times[0][0]);
			
			if (currentGlobexWeek == Globals.MinDate)
			{
				currentGlobexWeek = globexWeekEnd;
			}
			
			// Check if we've moved to a new Globex week
			if (globexWeekEnd != currentGlobexWeek)
			{
				// Reset for new Globex week
				globexHigh = high;
				globexLow = low;
				currentGlobexWeek = globexWeekEnd;
			}
			else
			{
				// Update current Globex week high/low
				globexHigh = Math.Max(globexHigh, high);
				globexLow = Math.Min(globexLow, low);
			}

			// Track RTH session (9:30am - 4pm EST, Monday-Friday)
			// RTH shows CURRENT day's range, not previous
			TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
			DateTime estTime = TimeZoneInfo.ConvertTime(Times[0][0], Bars.TradingHours.TimeZoneInfo, estZone);
			DateTime rthDayEnd = GetRTHDayEnd(Times[0][0]);
			
			if (currentRTHDay == Globals.MinDate)
			{
				currentRTHDay = rthDayEnd;
				currentRTHHigh = double.MinValue;
				currentRTHLow = double.MaxValue;
			}
			
			// Check if we've moved to a new RTH day
			if (rthDayEnd != currentRTHDay)
			{
				// Reset for new RTH day
				currentRTHHigh = double.MinValue;
				currentRTHLow = double.MaxValue;
				currentRTHDay = rthDayEnd;
			}
			
			// Update current RTH session high/low only if we're within RTH hours
			// Only track if between 9:30am and 4pm EST on weekdays
			if (estTime.DayOfWeek >= DayOfWeek.Monday && estTime.DayOfWeek <= DayOfWeek.Friday &&
			    estTime.TimeOfDay >= new TimeSpan(9, 30, 0) && estTime.TimeOfDay < new TimeSpan(16, 0, 0))
			{
				if (currentRTHHigh == double.MinValue)
					currentRTHHigh = high;
				else
					currentRTHHigh = Math.Max(currentRTHHigh, high);
					
				if (currentRTHLow == double.MaxValue)
					currentRTHLow = low;
				else
					currentRTHLow = Math.Min(currentRTHLow, low);
			}

			if ((pivotRangeType == RedTailPivotRange.Daily && currentDate != Globals.MinDate)
				|| (pivotRangeType == RedTailPivotRange.Weekly && currentWeek != Globals.MinDate)
				|| (pivotRangeType == RedTailPivotRange.Monthly && currentMonth != Globals.MinDate))
			{
				Pp[0] = pp;
				R1[0] = r1;
				R2[0] = r2;
				R3[0] = r3;
				S1[0] = s1;
				S2[0] = s2;
				S3[0] = s3;
				R1M[0] = r1m;
				R2M[0] = r2m;
				R3M[0] = r3m;
				S1M[0] = s1m;
				S2M[0] = s2m;
				S3M[0] = s3m;
				PDH[0] = ph;
				PDL[0] = pl;
				
				// Set PWH and PWL if we have valid values
				if (pwh != double.MinValue)
					PWH[0] = pwh;
				if (pwl != double.MaxValue)
					PWL[0] = pwl;
					
				// Set PMH and PML if we have valid values
				if (pmh != double.MinValue)
					PMH[0] = pmh;
				if (pml != double.MaxValue)
					PML[0] = pml;
					
				// Set Monday range if we have valid values
				if (mondayHigh != double.MinValue)
					MH[0] = mondayHigh;
				if (mondayLow != double.MaxValue)
					ML[0] = mondayLow;
					
				// Set Globex session range
				if (globexHigh != double.MinValue)
					GH[0] = globexHigh;
				if (globexLow != double.MaxValue)
					GL[0] = globexLow;
					
				// Set RTH session range (current day)
				if (currentRTHHigh != double.MinValue)
					NYH[0] = currentRTHHigh;
				if (currentRTHLow != double.MaxValue)
					NYL[0] = currentRTHLow;
					
				// Calculate and set Fibonacci levels based on previous day high/low
				if (ph != double.MinValue && pl != double.MaxValue)
				{
					double range = ph - pl;
					
					// Calculate each fib level if it's not 0.0 (0.0 means disabled)
					if (Fib1 > 0)
						FibLevel1[0] = pl + (range * Fib1);
					if (Fib2 > 0)
						FibLevel2[0] = pl + (range * Fib2);
					if (Fib3 > 0)
						FibLevel3[0] = pl + (range * Fib3);
					if (Fib4 > 0)
						FibLevel4[0] = pl + (range * Fib4);
					if (Fib5 > 0)
						FibLevel5[0] = pl + (range * Fib5);
					if (Fib6 > 0)
						FibLevel6[0] = pl + (range * Fib6);
					if (Fib7 > 0)
						FibLevel7[0] = pl + (range * Fib7);
					if (Fib8 > 0)
						FibLevel8[0] = pl + (range * Fib8);
					if (Fib9 > 0)
						FibLevel9[0] = pl + (range * Fib9);
					if (Fib10 > 0)
						FibLevel10[0] = pl + (range * Fib10);
				}
			}
		}

		#region Misc
		private DateTime GetLastBarSessionDate(DateTime time, RedTailPivotRange RedTailPivotRange)
		{
			// Check the time[0] against the previous session end
			if (time > cacheSessionEnd)
			{
				if (Bars.BarsType.IsIntraday)
				{
					// Make use of the stored session iterator to find the next session...
					storedSession.GetNextSession(time, true);
					// Store the actual session's end datetime as the session
					cacheSessionEnd = storedSession.ActualSessionEnd;
					// We need to convert that time from the session to the users time zone settings
					sessionDateTmp = TimeZoneInfo.ConvertTime(cacheSessionEnd.AddSeconds(-1), Globals.GeneralOptions.TimeZoneInfo, Bars.TradingHours.TimeZoneInfo).Date;
				}
				else
					sessionDateTmp = time.Date;
			}

			if (RedTailPivotRange == RedTailPivotRange.Daily)
			{
				if (sessionDateTmp != cacheSessionDate)
				{
					if (newSessionBarIdxArr.Count == 0 || newSessionBarIdxArr.Count > 0 && CurrentBar > newSessionBarIdxArr[newSessionBarIdxArr.Count - 1])
						newSessionBarIdxArr.Add(CurrentBar);
					cacheSessionDate = sessionDateTmp;
				}
				return sessionDateTmp;
			}

			DateTime tmpWeeklyEndDate = RoundUpTimeToPeriodTime(sessionDateTmp, RedTailPivotRange.Weekly);
			if (RedTailPivotRange == RedTailPivotRange.Weekly)
			{
				if (tmpWeeklyEndDate != cacheWeeklyEndDate)
				{
					if (newSessionBarIdxArr.Count == 0 || newSessionBarIdxArr.Count > 0 && CurrentBar > newSessionBarIdxArr[newSessionBarIdxArr.Count - 1])
						newSessionBarIdxArr.Add(CurrentBar);
					cacheWeeklyEndDate = tmpWeeklyEndDate;
				}
				return tmpWeeklyEndDate;
			}

			DateTime tmpMonthlyEndDate = RoundUpTimeToPeriodTime(sessionDateTmp, RedTailPivotRange.Monthly);
			if (tmpMonthlyEndDate != cacheMonthlyEndDate)
			{
				if (newSessionBarIdxArr.Count == 0 || newSessionBarIdxArr.Count > 0 && CurrentBar > newSessionBarIdxArr[newSessionBarIdxArr.Count - 1])
					newSessionBarIdxArr.Add(CurrentBar);
				cacheMonthlyEndDate = tmpMonthlyEndDate;
			}
			return tmpMonthlyEndDate;
		}

		private DateTime RoundUpTimeToPeriodTime(DateTime time, RedTailPivotRange RedTailPivotRange)
		{
			if (RedTailPivotRange == RedTailPivotRange.Weekly)
			{
				DateTime periodStart = time.AddDays((6 - (((int) time.DayOfWeek) + 1) % 7));
				return periodStart.Date.AddDays(Math.Ceiling(Math.Ceiling(time.Date.Subtract(periodStart.Date).TotalDays)/7)*7).Date;
			}
			if (RedTailPivotRange == RedTailPivotRange.Monthly)
			{
				var result = new DateTime(time.Year, time.Month, 1);
				return result.AddMonths(1).AddDays(-1);
			}
			return time;
		}

		private DateTime GetWeekEndDate(DateTime time)
		{
			// Convert to session time if needed
			DateTime sessionTime = time;
			if (Bars.BarsType.IsIntraday && storedSession != null)
			{
				sessionTime = TimeZoneInfo.ConvertTime(time, Bars.TradingHours.TimeZoneInfo, Globals.GeneralOptions.TimeZoneInfo);
			}
			
			// A trading week runs from Sunday 6pm to Friday 5pm
			// Adjust the time to account for the 6pm start
			DateTime adjustedTime = sessionTime;
			if (sessionTime.DayOfWeek == DayOfWeek.Sunday && sessionTime.TimeOfDay < new TimeSpan(18, 0, 0))
			{
				// Before 6pm Sunday, we're still in previous week
				adjustedTime = sessionTime.AddDays(-1);
			}
			else if (sessionTime.DayOfWeek == DayOfWeek.Friday && sessionTime.TimeOfDay >= new TimeSpan(17, 0, 0))
			{
				// After 5pm Friday, we've started the next week
				adjustedTime = sessionTime.AddDays(1);
			}
			else if (sessionTime.DayOfWeek == DayOfWeek.Saturday)
			{
				// Saturday is part of next week (since week ended Friday 5pm)
				adjustedTime = sessionTime.AddDays(1);
			}
			
			// Find the upcoming Friday
			int daysUntilFriday = ((int)DayOfWeek.Friday - (int)adjustedTime.DayOfWeek + 7) % 7;
			if (daysUntilFriday == 0 && adjustedTime.TimeOfDay >= new TimeSpan(17, 0, 0))
			{
				// Already past 5pm Friday, move to next Friday
				daysUntilFriday = 7;
			}
			
			DateTime friday = adjustedTime.Date.AddDays(daysUntilFriday);
			return friday.Date.AddHours(17); // 5pm Friday
		}

		private DateTime GetMonthEndDate(DateTime time)
		{
			// Convert to session time if needed
			DateTime sessionTime = time;
			if (Bars.BarsType.IsIntraday && storedSession != null)
			{
				sessionTime = TimeZoneInfo.ConvertTime(time, Bars.TradingHours.TimeZoneInfo, Globals.GeneralOptions.TimeZoneInfo);
			}
			
			// A trading month runs from 6pm on the last TRADING day of previous month
			// to 5pm on the last TRADING day of current month
			// Example: If Nov 30 is Saturday, November trading month ends Nov 28 (Friday) at 5pm
			//          and December trading month starts Nov 28 at 6pm (after Nov close)
			
			DateTime currentMonth = sessionTime.Date;
			
			// Get the last trading day of current calendar month
			DateTime lastCalendarDay = new DateTime(currentMonth.Year, currentMonth.Month, 1).AddMonths(1).AddDays(-1);
			DateTime lastTradingDay = GetLastTradingDayOfMonth(lastCalendarDay);
			
			// Determine which trading month we're in
			if (sessionTime.Date.Month != currentMonth.Month)
			{
				// We're in a different month, need to recalculate
				currentMonth = sessionTime.Date;
				lastCalendarDay = new DateTime(currentMonth.Year, currentMonth.Month, 1).AddMonths(1).AddDays(-1);
				lastTradingDay = GetLastTradingDayOfMonth(lastCalendarDay);
			}
			
			// Check if we're past the current month's close (after 5pm on last trading day)
			if (sessionTime.Date > lastTradingDay || 
			    (sessionTime.Date == lastTradingDay && sessionTime.TimeOfDay >= new TimeSpan(17, 0, 0)))
			{
				// We're in next trading month
				currentMonth = currentMonth.AddMonths(1);
				lastCalendarDay = new DateTime(currentMonth.Year, currentMonth.Month, 1).AddMonths(1).AddDays(-1);
				lastTradingDay = GetLastTradingDayOfMonth(lastCalendarDay);
			}
			
			return lastTradingDay.Date.AddHours(17); // 5pm on last trading day of month
		}

		private DateTime GetLastTradingDayOfMonth(DateTime monthEnd)
		{
			// Start with the last day of the month
			DateTime lastDay = monthEnd;
			
			// Walk backwards to find last weekday (Friday or earlier)
			while (lastDay.DayOfWeek == DayOfWeek.Saturday || lastDay.DayOfWeek == DayOfWeek.Sunday)
			{
				lastDay = lastDay.AddDays(-1);
			}
			
			return lastDay;
		}

		private DateTime GetMondayWeekEnd(DateTime time)
		{
			// Convert to session time if needed
			DateTime sessionTime = time;
			if (Bars.BarsType.IsIntraday && storedSession != null)
			{
				sessionTime = TimeZoneInfo.ConvertTime(time, Bars.TradingHours.TimeZoneInfo, Globals.GeneralOptions.TimeZoneInfo);
			}
			
			// Monday session runs from Sunday 6pm to Monday 5pm
			// We need to determine which week's Monday we're in
			
			DateTime adjustedTime = sessionTime;
			
			// If it's Sunday before 6pm, we're still in the previous week's Monday session
			if (sessionTime.DayOfWeek == DayOfWeek.Sunday && sessionTime.TimeOfDay < new TimeSpan(18, 0, 0))
			{
				adjustedTime = sessionTime.AddDays(-7);
			}
			// If it's Monday after 5pm, we've finished this week's Monday session
			else if (sessionTime.DayOfWeek == DayOfWeek.Monday && sessionTime.TimeOfDay >= new TimeSpan(17, 0, 0))
			{
				// Already in next week
				adjustedTime = sessionTime.AddDays(7);
			}
			// If it's Tuesday through Saturday, we're past this week's Monday
			else if (sessionTime.DayOfWeek >= DayOfWeek.Tuesday)
			{
				// Move to next week's Monday
				adjustedTime = sessionTime.AddDays(7);
			}
			
			// Find the upcoming Monday from adjustedTime
			int daysUntilMonday = ((int)DayOfWeek.Monday - (int)adjustedTime.DayOfWeek + 7) % 7;
			if (daysUntilMonday == 0 && adjustedTime.TimeOfDay >= new TimeSpan(17, 0, 0))
			{
				// Already past 5pm Monday, move to next Monday
				daysUntilMonday = 7;
			}
			
			DateTime monday = adjustedTime.Date.AddDays(daysUntilMonday);
			return monday.Date.AddHours(17); // 5pm Monday
		}

		private DateTime GetRTHDayEnd(DateTime time)
		{
			// Convert to EST
			TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
			DateTime estTime = TimeZoneInfo.ConvertTime(time, Bars.TradingHours.TimeZoneInfo, estZone);
			
			// RTH is 9:30am - 4pm EST, Monday-Friday
			// Skip weekends
			DateTime currentDay = estTime.Date;
			
			// If it's before 9:30am or after 4pm, we need to find the next RTH day
			if (estTime.TimeOfDay < new TimeSpan(9, 30, 0))
			{
				// Before RTH open - today's 4pm is the end (if it's a weekday)
				if (estTime.DayOfWeek >= DayOfWeek.Monday && estTime.DayOfWeek <= DayOfWeek.Friday)
				{
					return currentDay.AddHours(16); // 4pm today
				}
				else
				{
					// Weekend - find next Monday
					int daysToAdd = ((int)DayOfWeek.Monday - (int)estTime.DayOfWeek + 7) % 7;
					return currentDay.AddDays(daysToAdd).AddHours(16);
				}
			}
			else if (estTime.TimeOfDay >= new TimeSpan(16, 0, 0))
			{
				// After RTH close - find next trading day
				if (estTime.DayOfWeek == DayOfWeek.Friday)
				{
					// Friday after close - next is Monday
					return currentDay.AddDays(3).AddHours(16);
				}
				else if (estTime.DayOfWeek == DayOfWeek.Saturday)
				{
					return currentDay.AddDays(2).AddHours(16);
				}
				else if (estTime.DayOfWeek == DayOfWeek.Sunday)
				{
					return currentDay.AddDays(1).AddHours(16);
				}
				else
				{
					// Monday-Thursday after close - next day
					return currentDay.AddDays(1).AddHours(16);
				}
			}
			else
			{
				// During RTH - today's 4pm is the end
				return currentDay.AddHours(16);
			}
		}

		protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
		{
			if (ShowHistorical)
				base.OnRender(chartControl, chartScale);
			// Set text to chart label color and font
			TextFormat	textFormat			= chartControl.Properties.LabelFont.ToDirectWriteTextFormat();

			// Build list of active levels with their values
			List<LevelInfo> activeLevels = new List<LevelInfo>();
			
			// Loop through each Plot Values on the chart
			for (int seriesCount = 0; seriesCount < Values.Length; seriesCount++)
			{
				// Skip based on toggle settings
				// Midlines (indices 7-12)
				if (!ShowMidlines && (seriesCount >= 7 && seriesCount <= 12))
					continue;
				
				// Pivots: PP, R1, R2, R3, S1, S2, S3 (indices 0-6)
				if (!ShowPivots && (seriesCount >= 0 && seriesCount <= 6))
					continue;
				
				// Previous Day: PH, PL (indices 13-14)
				if (!ShowPreviousDay && (seriesCount >= 13 && seriesCount <= 14))
					continue;
				
				// Previous Week: PWH, PWL (indices 15-16)
				if (!ShowPreviousWeek && (seriesCount >= 15 && seriesCount <= 16))
					continue;
				
				// Previous Month: PMH, PML (indices 17-18)
				if (!ShowPreviousMonth && (seriesCount >= 17 && seriesCount <= 18))
					continue;
				
				// Monday Range: MH, ML (indices 19-20)
				if (!ShowMondayRange && (seriesCount >= 19 && seriesCount <= 20))
					continue;
				
				// Globex Session: GH, GL (indices 21-22)
				if (!ShowGlobexSession && (seriesCount >= 21 && seriesCount <= 22))
					continue;
				
				// RTH Session: NYH, NYL (indices 23-24)
				if (!ShowRTHSession && (seriesCount >= 23 && seriesCount <= 24))
					continue;
				
				// Fibonacci Levels: Fib1-Fib10 (indices 25-34)
				if (!ShowFibLevels && (seriesCount >= 25 && seriesCount <= 34))
					continue;
				
				double	y					= -1;
				double	startX				= -1;
				double	endX				= -1;
				int		firstBarIdxToPaint	= -1;
				int		firstBarPainted		= ChartBars.FromIndex;
				int		lastBarPainted		= ChartBars.ToIndex;
				Plot	plot				= Plots[seriesCount];

				for (int i = newSessionBarIdxArr.Count - 1; i >= 0; i--)
				{
					int prevSessionBreakIdx = newSessionBarIdxArr[i];
					if (prevSessionBreakIdx <= lastBarPainted)
					{
						firstBarIdxToPaint = prevSessionBreakIdx;
						break;
					}
				}

				// Get the value for this level
				double val = 0;
				// If width is 0, extend back to first visible bar, otherwise limit by width
				int startIdx = width == 0 ? firstBarPainted : Math.Max(firstBarPainted, lastBarPainted - width);
				
				for (int idx = lastBarPainted; idx >= startIdx; idx--)
				{
					if (idx < firstBarIdxToPaint)
						break;

					startX		= chartControl.GetXByBarIndex(ChartBars, idx);
					endX		= chartControl.GetXByBarIndex(ChartBars, lastBarPainted);
					val			= Values[seriesCount].GetValueAt(idx);
					y			= chartScale.GetYByValue(val);
				}

				// Add to active levels list
				string levelName = plot.Name;
				
				// For Fibonacci levels, use the actual value instead of "Fib1", "Fib2", etc.
				if (seriesCount >= 25 && seriesCount <= 34) // Fib levels are indices 25-34
				{
					double fibValue = 0.0;
					switch (seriesCount)
					{
						case 25: fibValue = Fib1; break;
						case 26: fibValue = Fib2; break;
						case 27: fibValue = Fib3; break;
						case 28: fibValue = Fib4; break;
						case 29: fibValue = Fib5; break;
						case 30: fibValue = Fib6; break;
						case 31: fibValue = Fib7; break;
						case 32: fibValue = Fib8; break;
						case 33: fibValue = Fib9; break;
						case 34: fibValue = Fib10; break;
					}
					if (fibValue > 0)
						levelName = fibValue.ToString("0.000");
				}
				
				activeLevels.Add(new LevelInfo
				{
					SeriesIndex = seriesCount,
					Value = val,
					Y = y,
					StartX = startX,
					EndX = endX,
					Plot = plot,
					Name = levelName
				});
			}

			// Merge levels if tolerance is set
			if (MergeTolerance > 0)
			{
				List<LevelInfo> mergedLevels = new List<LevelInfo>();
				List<bool> processed = new List<bool>(new bool[activeLevels.Count]);
				
				for (int i = 0; i < activeLevels.Count; i++)
				{
					if (processed[i])
						continue;
					
					LevelInfo currentLevel = activeLevels[i];
					List<string> mergedNames = new List<string> { currentLevel.Name };
					
					// Find all levels within tolerance
					for (int j = i + 1; j < activeLevels.Count; j++)
					{
						if (processed[j])
							continue;
						
						if (Math.Abs(activeLevels[j].Value - currentLevel.Value) <= MergeTolerance * TickSize)
						{
							mergedNames.Add(activeLevels[j].Name);
							processed[j] = true;
						}
					}
					
					// Create merged level
					currentLevel.Name = string.Join("/", mergedNames);
					mergedLevels.Add(currentLevel);
					processed[i] = true;
				}
				
				activeLevels = mergedLevels;
			}

			// Draw all levels
			foreach (LevelInfo level in activeLevels)
			{
				// Draw pivot lines
				Point startPoint	= new Point(level.StartX, level.Y);
				Point endPoint		= new Point(level.EndX, level.Y);
				RenderTarget.DrawLine(startPoint.ToVector2(), endPoint.ToVector2(), level.Plot.BrushDX, level.Plot.Width, level.Plot.StrokeStyle);

				// Draw pivot text on the right side
				TextLayout textLayout = new TextLayout(Globals.DirectWriteFactory, level.Name, textFormat, ChartPanel.W, textFormat.FontSize);
				// Position text at the end of the line (right side)
				Point textPoint = new Point(endPoint.X + 5, endPoint.Y - textFormat.FontSize / 2);
				RenderTarget.DrawTextLayout(textPoint.ToVector2(), textLayout, level.Plot.BrushDX);
				textLayout.Dispose();
			}
			
			textFormat.Dispose();
		}

		// Helper class to store level information
		private class LevelInfo
		{
			public int SeriesIndex { get; set; }
			public double Value { get; set; }
			public double Y { get; set; }
			public double StartX { get; set; }
			public double EndX { get; set; }
			public Plot Plot { get; set; }
			public string Name { get; set; }
		}
		#endregion

		#region Properties
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Show Historical Pivots", GroupName = "NinjaScriptParameters", Order = 0)]
		public bool ShowHistorical
		{ get; set; }

		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "RedTailPivotRange", GroupName = "NinjaScriptParameters", Order = 1)]
		public RedTailPivotRange PivotRangeType
		{
			get { return pivotRangeType; }
			set { pivotRangeType = value; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Pp
		{
			get { return Values[0]; }
		}

		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "RedTailHLCMode", GroupName = "NinjaScriptParameters", Order = 2)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		public RedTailHLCMode PriorDayHlc
		{
			get { return priorDayHlc; }
			set { priorDayHlc = value; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> R1
		{
			get { return Values[1]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> R2
		{
			get { return Values[2]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> R3
		{
			get { return Values[3]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> S1
		{
			get { return Values[4]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> S2
		{
			get { return Values[5]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> S3
		{
			get { return Values[6]; }
		}
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> R1M
		{
			get { return Values[7]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> R2M
		{
			get { return Values[8]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> R3M
		{
			get { return Values[9]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> S1M
		{
			get { return Values[10]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> S2M
		{
			get { return Values[11]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> S3M
		{
			get { return Values[12]; }
		}
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> PDH
		{
			get { return Values[13]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> PDL
		{
			get { return Values[14]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> PWH
		{
			get { return Values[15]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> PWL
		{
			get { return Values[16]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> PMH
		{
			get { return Values[17]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> PML
		{
			get { return Values[18]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> MH
		{
			get { return Values[19]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> ML
		{
			get { return Values[20]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> GH
		{
			get { return Values[21]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> GL
		{
			get { return Values[22]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> NYH
		{
			get { return Values[23]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> NYL
		{
			get { return Values[24]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> FibLevel1
		{
			get { return Values[25]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> FibLevel2
		{
			get { return Values[26]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> FibLevel3
		{
			get { return Values[27]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> FibLevel4
		{
			get { return Values[28]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> FibLevel5
		{
			get { return Values[29]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> FibLevel6
		{
			get { return Values[30]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> FibLevel7
		{
			get { return Values[31]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> FibLevel8
		{
			get { return Values[32]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> FibLevel9
		{
			get { return Values[33]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> FibLevel10
		{
			get { return Values[34]; }
		}

		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "UserDefinedClose", GroupName = "NinjaScriptParameters", Order = 3)]
		public double UserDefinedClose
		{
			get { return userDefinedClose; }
			set { userDefinedClose = value; }
		}

		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "UserDefinedHigh", GroupName = "NinjaScriptParameters", Order = 4)]
		public double UserDefinedHigh
		{
			get { return userDefinedHigh; }
			set { userDefinedHigh = value; }
		}

		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "UserDefinedLow", GroupName = "NinjaScriptParameters", Order = 5)]
		public double UserDefinedLow
		{
			get { return userDefinedLow; }
			set { userDefinedLow = value; }
		}

		[NinjaScriptProperty]
		[Range(0, 1000)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Width", Description = "Number of bars to extend levels back (0 = infinite)", GroupName = "NinjaScriptParameters", Order = 6)]
		public int Width
		{
			get { return width; }
			set { width = value; }
		}

		[NinjaScriptProperty]
		[Display(Name = "Show Midlines", Description = "Show or hide midline pivot levels", GroupName = "NinjaScriptParameters", Order = 7)]
		public bool ShowMidlines
		{
			get { return showMidlines; }
			set { showMidlines = value; }
		}

		[NinjaScriptProperty]
		[Display(Name = "Show Pivots", Description = "Show or hide pivot levels (PP, R1-R3, S1-S3)", GroupName = "NinjaScriptParameters", Order = 8)]
		public bool ShowPivots
		{
			get { return showPivots; }
			set { showPivots = value; }
		}

		[NinjaScriptProperty]
		[Display(Name = "Show Previous Day", Description = "Show or hide previous day high and low", GroupName = "NinjaScriptParameters", Order = 9)]
		public bool ShowPreviousDay
		{
			get { return showPreviousDay; }
			set { showPreviousDay = value; }
		}

		[NinjaScriptProperty]
		[Display(Name = "Show Previous Week", Description = "Show or hide previous week high and low", GroupName = "NinjaScriptParameters", Order = 10)]
		public bool ShowPreviousWeek
		{
			get { return showPreviousWeek; }
			set { showPreviousWeek = value; }
		}

		[NinjaScriptProperty]
		[Display(Name = "Show Previous Month", Description = "Show or hide previous month high and low", GroupName = "NinjaScriptParameters", Order = 11)]
		public bool ShowPreviousMonth
		{
			get { return showPreviousMonth; }
			set { showPreviousMonth = value; }
		}

		[NinjaScriptProperty]
		[Display(Name = "Show Monday Range", Description = "Show or hide Monday's range (Sunday 6pm - Monday 5pm)", GroupName = "NinjaScriptParameters", Order = 12)]
		public bool ShowMondayRange
		{
			get { return showMondayRange; }
			set { showMondayRange = value; }
		}

		[NinjaScriptProperty]
		[Display(Name = "Show Globex Session", Description = "Show or hide Globex session range (Sunday 6pm - Friday 5pm)", GroupName = "NinjaScriptParameters", Order = 13)]
		public bool ShowGlobexSession
		{
			get { return showGlobexSession; }
			set { showGlobexSession = value; }
		}

		[NinjaScriptProperty]
		[Display(Name = "Show RTH Session", Description = "Show or hide RTH session range (9:30am - 4pm EST, Mon-Fri)", GroupName = "NinjaScriptParameters", Order = 14)]
		public bool ShowRTHSession
		{
			get { return showRTHSession; }
			set { showRTHSession = value; }
		}

		[NinjaScriptProperty]
		[Display(Name = "Merge Tolerance (Ticks)", Description = "Merge levels within this many ticks (0 = no merging)", GroupName = "NinjaScriptParameters", Order = 15)]
		public double MergeTolerance
		{
			get { return mergeTolerance; }
			set { mergeTolerance = Math.Max(0, value); }
		}

		[NinjaScriptProperty]
		[Display(Name = "Show Fibonacci Levels", Description = "Show or hide Fibonacci retracement levels", GroupName = "NinjaScriptParameters", Order = 16)]
		public bool ShowFibLevels
		{
			get { return showFibLevels; }
			set { showFibLevels = value; }
		}

		[NinjaScriptProperty]
		[Range(0.0, 1.0)]
		[Display(Name = "Fib Level 1", Description = "Fibonacci level 1 (0.0 = disabled, default 0.236)", GroupName = "Fibonacci Levels", Order = 1)]
		public double Fib1
		{
			get { return fib1; }
			set { fib1 = value; }
		}

		[NinjaScriptProperty]
		[Range(0.0, 1.0)]
		[Display(Name = "Fib Level 2", Description = "Fibonacci level 2 (0.0 = disabled, default 0.382)", GroupName = "Fibonacci Levels", Order = 2)]
		public double Fib2
		{
			get { return fib2; }
			set { fib2 = value; }
		}

		[NinjaScriptProperty]
		[Range(0.0, 1.0)]
		[Display(Name = "Fib Level 3", Description = "Fibonacci level 3 (0.0 = disabled, default 0.500)", GroupName = "Fibonacci Levels", Order = 3)]
		public double Fib3
		{
			get { return fib3; }
			set { fib3 = value; }
		}

		[NinjaScriptProperty]
		[Range(0.0, 1.0)]
		[Display(Name = "Fib Level 4", Description = "Fibonacci level 4 (0.0 = disabled, default 0.618)", GroupName = "Fibonacci Levels", Order = 4)]
		public double Fib4
		{
			get { return fib4; }
			set { fib4 = value; }
		}

		[NinjaScriptProperty]
		[Range(0.0, 1.0)]
		[Display(Name = "Fib Level 5", Description = "Fibonacci level 5 (0.0 = disabled, default 0.786)", GroupName = "Fibonacci Levels", Order = 5)]
		public double Fib5
		{
			get { return fib5; }
			set { fib5 = value; }
		}

		[NinjaScriptProperty]
		[Range(0.0, 1.0)]
		[Display(Name = "Fib Level 6", Description = "Fibonacci level 6 (0.0 = disabled)", GroupName = "Fibonacci Levels", Order = 6)]
		public double Fib6
		{
			get { return fib6; }
			set { fib6 = value; }
		}

		[NinjaScriptProperty]
		[Range(0.0, 1.0)]
		[Display(Name = "Fib Level 7", Description = "Fibonacci level 7 (0.0 = disabled)", GroupName = "Fibonacci Levels", Order = 7)]
		public double Fib7
		{
			get { return fib7; }
			set { fib7 = value; }
		}

		[NinjaScriptProperty]
		[Range(0.0, 1.0)]
		[Display(Name = "Fib Level 8", Description = "Fibonacci level 8 (0.0 = disabled)", GroupName = "Fibonacci Levels", Order = 8)]
		public double Fib8
		{
			get { return fib8; }
			set { fib8 = value; }
		}

		[NinjaScriptProperty]
		[Range(0.0, 1.0)]
		[Display(Name = "Fib Level 9", Description = "Fibonacci level 9 (0.0 = disabled)", GroupName = "Fibonacci Levels", Order = 9)]
		public double Fib9
		{
			get { return fib9; }
			set { fib9 = value; }
		}

		[NinjaScriptProperty]
		[Range(0.0, 1.0)]
		[Display(Name = "Fib Level 10", Description = "Fibonacci level 10 (0.0 = disabled)", GroupName = "Fibonacci Levels", Order = 10)]
		public double Fib10
		{
			get { return fib10; }
			set { fib10 = value; }
		}

		#endregion
	}

	// Hide UserDefinedValues properties when not in use by the RedTailHLCMode.UserDefinedValues
	// When creating a custom type converter for indicators it must inherit from NinjaTrader.NinjaScript.IndicatorBaseConverter to work correctly with indicators
	public class RedTailKeyLevelsTypeConverter : NinjaTrader.NinjaScript.IndicatorBaseConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) { return true; }

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = base.GetPropertiesSupported(context) ? base.GetProperties(context, value, attributes) : TypeDescriptor.GetProperties(value, attributes);

			RedTailKeyLevels						thisPivotsInstance			= (RedTailKeyLevels) value;
			RedTailHLCMode	selectedRedTailHLCMode	= thisPivotsInstance.PriorDayHlc;
			if (selectedRedTailHLCMode == RedTailHLCMode.UserDefinedValues)
				return propertyDescriptorCollection;

			PropertyDescriptorCollection adjusted = new PropertyDescriptorCollection(null);
			foreach (PropertyDescriptor thisDescriptor in propertyDescriptorCollection)
			{
				if (thisDescriptor.Name == "UserDefinedClose" || thisDescriptor.Name == "UserDefinedHigh" || thisDescriptor.Name == "UserDefinedLow")
					adjusted.Add(new PropertyDescriptorExtended(thisDescriptor, o => value, null, new Attribute[] {new BrowsableAttribute(false), }));
				else
					adjusted.Add(thisDescriptor);
			}
			return adjusted;
		}
	}
}

[TypeConverter("NinjaTrader.Custom.ResourceEnumConverter")]
public enum RedTailHLCMode
{
	CalcFromIntradayData,
	DailyBars,
	UserDefinedValues
}

[TypeConverter("NinjaTrader.Custom.ResourceEnumConverter")]
public enum RedTailPivotRange
{
	Daily,
	Weekly,
	Monthly,
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private RedTailKeyLevels[] cacheRedTailKeyLevels;
		public RedTailKeyLevels RedTailKeyLevels(bool showHistorical, RedTailPivotRange pivotRangeType, RedTailHLCMode priorDayHlc, double userDefinedClose, double userDefinedHigh, double userDefinedLow, int width, bool showMidlines, bool showPivots, bool showPreviousDay, bool showPreviousWeek, bool showPreviousMonth, bool showMondayRange, bool showGlobexSession, bool showRTHSession, double mergeTolerance, bool showFibLevels, double fib1, double fib2, double fib3, double fib4, double fib5, double fib6, double fib7, double fib8, double fib9, double fib10)
		{
			return RedTailKeyLevels(Input, showHistorical, pivotRangeType, priorDayHlc, userDefinedClose, userDefinedHigh, userDefinedLow, width, showMidlines, showPivots, showPreviousDay, showPreviousWeek, showPreviousMonth, showMondayRange, showGlobexSession, showRTHSession, mergeTolerance, showFibLevels, fib1, fib2, fib3, fib4, fib5, fib6, fib7, fib8, fib9, fib10);
		}

		public RedTailKeyLevels RedTailKeyLevels(ISeries<double> input, bool showHistorical, RedTailPivotRange pivotRangeType, RedTailHLCMode priorDayHlc, double userDefinedClose, double userDefinedHigh, double userDefinedLow, int width, bool showMidlines, bool showPivots, bool showPreviousDay, bool showPreviousWeek, bool showPreviousMonth, bool showMondayRange, bool showGlobexSession, bool showRTHSession, double mergeTolerance, bool showFibLevels, double fib1, double fib2, double fib3, double fib4, double fib5, double fib6, double fib7, double fib8, double fib9, double fib10)
		{
			if (cacheRedTailKeyLevels != null)
				for (int idx = 0; idx < cacheRedTailKeyLevels.Length; idx++)
					if (cacheRedTailKeyLevels[idx] != null && cacheRedTailKeyLevels[idx].ShowHistorical == showHistorical && cacheRedTailKeyLevels[idx].PivotRangeType == pivotRangeType && cacheRedTailKeyLevels[idx].PriorDayHlc == priorDayHlc && cacheRedTailKeyLevels[idx].UserDefinedClose == userDefinedClose && cacheRedTailKeyLevels[idx].UserDefinedHigh == userDefinedHigh && cacheRedTailKeyLevels[idx].UserDefinedLow == userDefinedLow && cacheRedTailKeyLevels[idx].Width == width && cacheRedTailKeyLevels[idx].ShowMidlines == showMidlines && cacheRedTailKeyLevels[idx].ShowPivots == showPivots && cacheRedTailKeyLevels[idx].ShowPreviousDay == showPreviousDay && cacheRedTailKeyLevels[idx].ShowPreviousWeek == showPreviousWeek && cacheRedTailKeyLevels[idx].ShowPreviousMonth == showPreviousMonth && cacheRedTailKeyLevels[idx].ShowMondayRange == showMondayRange && cacheRedTailKeyLevels[idx].ShowGlobexSession == showGlobexSession && cacheRedTailKeyLevels[idx].ShowRTHSession == showRTHSession && cacheRedTailKeyLevels[idx].MergeTolerance == mergeTolerance && cacheRedTailKeyLevels[idx].ShowFibLevels == showFibLevels && cacheRedTailKeyLevels[idx].Fib1 == fib1 && cacheRedTailKeyLevels[idx].Fib2 == fib2 && cacheRedTailKeyLevels[idx].Fib3 == fib3 && cacheRedTailKeyLevels[idx].Fib4 == fib4 && cacheRedTailKeyLevels[idx].Fib5 == fib5 && cacheRedTailKeyLevels[idx].Fib6 == fib6 && cacheRedTailKeyLevels[idx].Fib7 == fib7 && cacheRedTailKeyLevels[idx].Fib8 == fib8 && cacheRedTailKeyLevels[idx].Fib9 == fib9 && cacheRedTailKeyLevels[idx].Fib10 == fib10 && cacheRedTailKeyLevels[idx].EqualsInput(input))
						return cacheRedTailKeyLevels[idx];
			return CacheIndicator<RedTailKeyLevels>(new RedTailKeyLevels(){ ShowHistorical = showHistorical, PivotRangeType = pivotRangeType, PriorDayHlc = priorDayHlc, UserDefinedClose = userDefinedClose, UserDefinedHigh = userDefinedHigh, UserDefinedLow = userDefinedLow, Width = width, ShowMidlines = showMidlines, ShowPivots = showPivots, ShowPreviousDay = showPreviousDay, ShowPreviousWeek = showPreviousWeek, ShowPreviousMonth = showPreviousMonth, ShowMondayRange = showMondayRange, ShowGlobexSession = showGlobexSession, ShowRTHSession = showRTHSession, MergeTolerance = mergeTolerance, ShowFibLevels = showFibLevels, Fib1 = fib1, Fib2 = fib2, Fib3 = fib3, Fib4 = fib4, Fib5 = fib5, Fib6 = fib6, Fib7 = fib7, Fib8 = fib8, Fib9 = fib9, Fib10 = fib10 }, input, ref cacheRedTailKeyLevels);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.RedTailKeyLevels RedTailKeyLevels(bool showHistorical, RedTailPivotRange pivotRangeType, RedTailHLCMode priorDayHlc, double userDefinedClose, double userDefinedHigh, double userDefinedLow, int width, bool showMidlines, bool showPivots, bool showPreviousDay, bool showPreviousWeek, bool showPreviousMonth, bool showMondayRange, bool showGlobexSession, bool showRTHSession, double mergeTolerance, bool showFibLevels, double fib1, double fib2, double fib3, double fib4, double fib5, double fib6, double fib7, double fib8, double fib9, double fib10)
		{
			return indicator.RedTailKeyLevels(Input, showHistorical, pivotRangeType, priorDayHlc, userDefinedClose, userDefinedHigh, userDefinedLow, width, showMidlines, showPivots, showPreviousDay, showPreviousWeek, showPreviousMonth, showMondayRange, showGlobexSession, showRTHSession, mergeTolerance, showFibLevels, fib1, fib2, fib3, fib4, fib5, fib6, fib7, fib8, fib9, fib10);
		}

		public Indicators.RedTailKeyLevels RedTailKeyLevels(ISeries<double> input , bool showHistorical, RedTailPivotRange pivotRangeType, RedTailHLCMode priorDayHlc, double userDefinedClose, double userDefinedHigh, double userDefinedLow, int width, bool showMidlines, bool showPivots, bool showPreviousDay, bool showPreviousWeek, bool showPreviousMonth, bool showMondayRange, bool showGlobexSession, bool showRTHSession, double mergeTolerance, bool showFibLevels, double fib1, double fib2, double fib3, double fib4, double fib5, double fib6, double fib7, double fib8, double fib9, double fib10)
		{
			return indicator.RedTailKeyLevels(input, showHistorical, pivotRangeType, priorDayHlc, userDefinedClose, userDefinedHigh, userDefinedLow, width, showMidlines, showPivots, showPreviousDay, showPreviousWeek, showPreviousMonth, showMondayRange, showGlobexSession, showRTHSession, mergeTolerance, showFibLevels, fib1, fib2, fib3, fib4, fib5, fib6, fib7, fib8, fib9, fib10);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.RedTailKeyLevels RedTailKeyLevels(bool showHistorical, RedTailPivotRange pivotRangeType, RedTailHLCMode priorDayHlc, double userDefinedClose, double userDefinedHigh, double userDefinedLow, int width, bool showMidlines, bool showPivots, bool showPreviousDay, bool showPreviousWeek, bool showPreviousMonth, bool showMondayRange, bool showGlobexSession, bool showRTHSession, double mergeTolerance, bool showFibLevels, double fib1, double fib2, double fib3, double fib4, double fib5, double fib6, double fib7, double fib8, double fib9, double fib10)
		{
			return indicator.RedTailKeyLevels(Input, showHistorical, pivotRangeType, priorDayHlc, userDefinedClose, userDefinedHigh, userDefinedLow, width, showMidlines, showPivots, showPreviousDay, showPreviousWeek, showPreviousMonth, showMondayRange, showGlobexSession, showRTHSession, mergeTolerance, showFibLevels, fib1, fib2, fib3, fib4, fib5, fib6, fib7, fib8, fib9, fib10);
		}

		public Indicators.RedTailKeyLevels RedTailKeyLevels(ISeries<double> input , bool showHistorical, RedTailPivotRange pivotRangeType, RedTailHLCMode priorDayHlc, double userDefinedClose, double userDefinedHigh, double userDefinedLow, int width, bool showMidlines, bool showPivots, bool showPreviousDay, bool showPreviousWeek, bool showPreviousMonth, bool showMondayRange, bool showGlobexSession, bool showRTHSession, double mergeTolerance, bool showFibLevels, double fib1, double fib2, double fib3, double fib4, double fib5, double fib6, double fib7, double fib8, double fib9, double fib10)
		{
			return indicator.RedTailKeyLevels(input, showHistorical, pivotRangeType, priorDayHlc, userDefinedClose, userDefinedHigh, userDefinedLow, width, showMidlines, showPivots, showPreviousDay, showPreviousWeek, showPreviousMonth, showMondayRange, showGlobexSession, showRTHSession, mergeTolerance, showFibLevels, fib1, fib2, fib3, fib4, fib5, fib6, fib7, fib8, fib9, fib10);
		}
	}
}

#endregion
