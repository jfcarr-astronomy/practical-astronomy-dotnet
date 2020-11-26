using System;
using PALib.Helpers;

namespace PALib
{
	public class PASun
	{
		/// <summary>
		/// Calculate approximate position of the sun for a local date and time.
		/// </summary>
		/// <param name="lctHours">Local civil time, in hours.</param>
		/// <param name="lctMinutes">Local civil time, in minutes.</param>
		/// <param name="lctSeconds">Local civil time, in seconds.</param>
		/// <param name="localDay">Local day, day part.</param>
		/// <param name="localMonth">Local day, month part.</param>
		/// <param name="localYear">Local day, year part.</param>
		/// <param name="isDaylightSaving">Is daylight savings in effect?</param>
		/// <param name="zoneCorrection">Time zone correction, in hours.</param>
		/// <returns>
		/// <para>sunRAHour -- Right Ascension of Sun, hour part</para>
		/// <para>sunRAMin -- Right Ascension of Sun, minutes part</para>
		/// <para>sunRASec -- Right Ascension of Sun, seconds part</para>
		/// <para>sunDecDeg -- Declination of Sun, degrees part</para>
		/// <para>sunDecMin -- Declination of Sun, minutes part</para>
		/// <para>sunDecSec -- Declination of Sun, seconds part</para>
		/// </returns>
		public (double sunRAHour, double sunRAMin, double sunRASec, double sunDecDeg, double sunDecMin, double sunDecSec) ApproximatePositionOfSun(double lctHours, double lctMinutes, double lctSeconds, double localDay, int localMonth, int localYear, bool isDaylightSaving, int zoneCorrection)
		{
			var daylightSaving = (isDaylightSaving == true) ? 1 : 0;

			var greenwichDateDay = PAMacros.LocalCivilTimeGreenwichDay(lctHours, lctMinutes, lctSeconds, daylightSaving, zoneCorrection, localDay, localMonth, localYear);
			var greenwichDateMonth = PAMacros.LocalCivilTimeGreenwichMonth(lctHours, lctMinutes, lctSeconds, daylightSaving, zoneCorrection, localDay, localMonth, localYear);
			var greenwichDateYear = PAMacros.LocalCivilTimeGreenwichYear(lctHours, lctMinutes, lctSeconds, daylightSaving, zoneCorrection, localDay, localMonth, localYear);
			var utHours = PAMacros.LocalCivilTimeToUniversalTime(lctHours, lctMinutes, lctSeconds, daylightSaving, zoneCorrection, localDay, localMonth, localYear);
			var utDays = utHours / 24;
			var jdDays = PAMacros.CivilDateToJulianDate(greenwichDateDay, greenwichDateMonth, greenwichDateYear) + utDays;
			var dDays = jdDays - PAMacros.CivilDateToJulianDate(0, 1, 2010);
			var nDeg = 360 * dDays / 365.242191;
			var mDeg1 = nDeg + PAMacros.SunELong(0, 1, 2010) - PAMacros.SunPeri(0, 1, 2010);
			var mDeg2 = mDeg1 - 360 * (mDeg1 / 360).Floor();
			var eCDeg = 360 * PAMacros.SunEcc(0, 1, 2010) * mDeg2.ToRadians().Sine() / Math.PI;
			var lSDeg1 = nDeg + eCDeg + PAMacros.SunELong(0, 1, 2010);
			var lSDeg2 = lSDeg1 - 360 * (lSDeg1 / 360).Floor();
			var raDeg = PAMacros.EcRA(lSDeg2, 0, 0, 0, 0, 0, greenwichDateDay, greenwichDateMonth, greenwichDateYear);
			var raHours = PAMacros.DecimalDegreesToDegreeHours(raDeg);
			var decDeg = PAMacros.EcDec(lSDeg2, 0, 0, 0, 0, 0, greenwichDateDay, greenwichDateMonth, greenwichDateYear);

			var sunRAHour = PAMacros.DecimalHoursHour(raHours);
			var sunRAMin = PAMacros.DecimalHoursMinute(raHours);
			var sunRASec = PAMacros.DecimalHoursSecond(raHours);
			var sunDecDeg = PAMacros.DecimalDegreesDegrees(decDeg);
			var sunDecMin = PAMacros.DecimalDegreesMinutes(decDeg);
			var sunDecSec = PAMacros.DecimalDegreesSeconds(decDeg);

			return (sunRAHour, sunRAMin, sunRASec, sunDecDeg, sunDecMin, sunDecSec);
		}

		/// <summary>
		/// Calculate precise position of the sun for a local date and time.
		/// </summary>
		/// <param name="lctHours"></param>
		/// <param name="lctMinutes"></param>
		/// <param name="lctSeconds"></param>
		/// <param name="localDay"></param>
		/// <param name="localMonth"></param>
		/// <param name="localYear"></param>
		/// <param name="isDaylightSaving"></param>
		/// <param name="zoneCorrection"></param>
		/// <returns></returns>
		public (double sunRAHour, double sunRAMin, double sunRASec, double sunDecDeg, double sunDecMin, double sunDecSec) PrecisePositionOfSun(double lctHours, double lctMinutes, double lctSeconds, double localDay, int localMonth, int localYear, bool isDaylightSaving, int zoneCorrection)
		{
			var daylightSaving = (isDaylightSaving == true) ? 1 : 0;

			var gDay = PAMacros.LocalCivilTimeGreenwichDay(lctHours, lctMinutes, lctSeconds, daylightSaving, zoneCorrection, localDay, localMonth, localYear);
			var gMonth = PAMacros.LocalCivilTimeGreenwichMonth(lctHours, lctMinutes, lctSeconds, daylightSaving, zoneCorrection, localDay, localMonth, localYear);
			var gYear = PAMacros.LocalCivilTimeGreenwichYear(lctHours, lctMinutes, lctSeconds, daylightSaving, zoneCorrection, localDay, localMonth, localYear);
			var sunEclipticLongitudeDeg = PAMacros.SunLong(lctHours, lctMinutes, lctSeconds, daylightSaving, zoneCorrection, localDay, localMonth, localYear);
			var raDeg = PAMacros.EcRA(sunEclipticLongitudeDeg, 0, 0, 0, 0, 0, gDay, gMonth, gYear);
			var raHours = PAMacros.DecimalDegreesToDegreeHours(raDeg);
			var decDeg = PAMacros.EcDec(sunEclipticLongitudeDeg, 0, 0, 0, 0, 0, gDay, gMonth, gYear);

			var sunRAHour = PAMacros.DecimalHoursHour(raHours);
			var sunRAMin = PAMacros.DecimalHoursMinute(raHours);
			var sunRASec = PAMacros.DecimalHoursSecond(raHours);
			var sunDecDeg = PAMacros.DecimalDegreesDegrees(decDeg);
			var sunDecMin = PAMacros.DecimalDegreesMinutes(decDeg);
			var sunDecSec = PAMacros.DecimalDegreesSeconds(decDeg);

			return (sunRAHour, sunRAMin, sunRASec, sunDecDeg, sunDecMin, sunDecSec);
		}
	}
}