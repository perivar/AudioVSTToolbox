///
/// <summary> * org.hermit.android.instrument: graphical instruments for Android.
/// * <br>Copyright 2009 Ian Cameron Smith
/// * 
/// * <p>These classes provide input and display functions for creating on-screen
/// * instruments of various kinds in Android apps.
/// *
/// * <p>This program is free software; you can redistribute it and/or modify
/// * it under the terms of the GNU General Public License version 2
/// * as published by the Free Software Foundation (see COPYING).
/// * 
/// * <p>This program is distributed in the hope that it will be useful,
/// * but WITHOUT ANY WARRANTY; without even the implied warranty of
/// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
/// * GNU General Public License for more details. </summary>
/// 

using System;

namespace Wave2Zebra2Preset.HermitGauges
{

	///
	/// <summary> * An instrument which measures some quantity, or accesses or produces some
	/// * data, which can be displayed on one or more <seealso cref="Gauge"/> objects. </summary>
	/// 
	public class Instrument
	{

		// ******************************************************************** //
		// Constructor.
		// ******************************************************************** //

		///
		///	 <summary> * Set up this view.
		///	 * 
		///	 * @param	parent			Parent surface. </summary>
		///
		public Instrument()
		{
			
		}


		// ******************************************************************** //
		// Run Control.
		// ******************************************************************** //

		///
		/// <summary> * The application is starting. Perform any initial set-up prior to
		/// * starting the application. </summary>
		///
		public virtual void appStart()
		{
		}


		///
		/// <summary> * We are starting the main run; start measurements. </summary>
		///
		public virtual void measureStart()
		{
		}


		///
		/// <summary> * We are stopping / pausing the run; stop measurements. </summary>
		///
		public virtual void measureStop()
		{
		}


		///
		/// <summary> * The application is closing down. Clean up any resources. </summary>
		///
		public virtual void appStop()
		{
		}


		// ******************************************************************** //
		// Main Loop.
		// ******************************************************************** //

		///
		/// <summary> * Update the state of the instrument for the current frame.
		/// * 
		/// * <p>Instruments may override this, and can use it to read the
		/// * current input state. This method is invoked in the main animation
		/// * loop -- i.e. frequently.
		/// *  </summary>
		/// * <param name="now">  Nominal time of the current frame in ms. </param>
		///
		protected internal virtual void doUpdate(long now)
		{
		}

		// ******************************************************************** //
		// Class Data.
		// ******************************************************************** //

		// Debugging tag.
		private const string TAG = "instrument";


		// ******************************************************************** //
		// Private Data.
		// ******************************************************************** //

	}

}