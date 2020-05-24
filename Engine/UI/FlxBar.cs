using Engine.Extensions;
using Engine.MathUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;

namespace Engine.UI
{
	/// <summary>
	/// FlxBar is a quick and easy way to create a graphical bar which can
	/// be used as part of your UI/HUD, or positioned next to a sprite.
	/// It could represent a loader, progress or health bar.
	/// </summary>
	public class FlxBar : FlxSprite
	{
		#region Public Properties
		/// <summary>
		/// If false, the bar is tracking its parent
		/// (the position is synchronized with the parent's position).
		/// </summary>
		public bool FixedPosition = true;
		/// <summary>
		/// How many pixels = 1% of the bar (barWidth (or barHeight) / 100)
		/// </summary>
		public float PxPerPercent { get; private set; }
		/// <summary>
		/// The positionOffset controls how far offset the FlxBar is from the parent sprite (if at all)
		/// </summary>
		public Vector2 PositionOffset { get; private set; }
		/// <summary>
		/// If this FlxBar should be killed when its empty
		/// </summary>
		public bool KillOnEmpty = false;
		/// <summary>
		/// The percentage of how full the bar is (a value between 0 and 100)
		/// </summary>
		public float Percent
		{
			get
			{
				if (Value.Value > Max)
				{
					return _maxPercent;
				}

				return (float)Math.Floor(((Value.Value - Min) / Range) * _maxPercent);
			}
			set
			{
				if (value >= 0 && value <= _maxPercent)
				{
					Value = Pct * value;
				}
			}
		}
		/// <summary>
		/// The current value - must always be between min and max
		/// </summary>
		public float? Value
		{
			get => _value.GetValueOrDefault();
			set
			{
				_value = Math.Max(Min, Math.Min(value.GetValueOrDefault(), Max));

				if (Value == Min)
				{
					EmptyCallback?.Invoke();
				}

				if (Value == Max)
				{
					FilledCallback?.Invoke();
				}

				if (Value == Min && KillOnEmpty)
				{
					Kill();
				}
				UpdateBar();
			}
		}
		private float? _value;
		/// <summary>
		/// The minimum value the bar can be (can never be >= max)
		/// </summary>
		public float Min { get; private set; }
		/// <summary>
		/// The maximum value the bar can be (can never be <= min)
		/// </summary>
		public float Max { get; private set; }
		/// <summary>
		/// How wide is the range of this bar? (max - min)
		/// </summary>
		public float Range { get; private set; }
		/// <summary>
		/// What 1% of the bar is equal to in terms of value (range / 100)
		/// </summary>
		public float Pct { get; private set; }
		/// <summary>
		/// Number of frames FlxBar will have. Default value is 100.
		/// The bigger value you set then visual will change smoother.
		/// </summary>
		public int NumDivisions
		{
			get => _numDivisions;
			set
			{
				_numDivisions = (value > 0) ? value : 100;
				UpdateFilledBar();
			}
		}
		private int _numDivisions = 100;
		/// <summary>
		/// This function will be called when value will hit it's minimum
		/// </summary>
		public Action EmptyCallback;
		/// <summary>
		/// This function will be called when value will hit it's maximum
		/// </summary>
		public Action FilledCallback;
		/// <summary>
		/// Object to track value from/
		/// </summary>
		public FlxObject Parent;
		/// <summary>
		/// Property of parent object to track.
		/// </summary>
		public string ParentVariable;

		public int BarWidth { get; private set; }
		public int BarHeight { get; private set; }
		/**
 * BarFrames which will be used for filled bar rendering.
 * It is recommended to use this property in tile render mode
 * (although it will work in blit render mode also).
 */
		//@:isVar
		//public FlxImageFrame frontFrames(get, set);

		//public FlxImageFrame backFrames(get, set):FlxImageFrame;

		/**
		 * The direction from which the health bar will fill-up. Default is from left to right. Change takes effect immediately.
		 */

		private FlxBarFillDirection _fillDirection;
		public FlxBarFillDirection FillDirection
		{
			get => _fillDirection;
			set
			{
				_fillDirection = value;

				switch (value)
				{
					case FlxBarFillDirection.LEFT_TO_RIGHT:
					case FlxBarFillDirection.RIGHT_TO_LEFT:
					case FlxBarFillDirection.HORIZONTAL_INSIDE_OUT:
					case FlxBarFillDirection.HORIZONTAL_OUTSIDE_IN:
						_fillHorizontal = true;
						break;

					case FlxBarFillDirection.TOP_TO_BOTTOM:
					case FlxBarFillDirection.BOTTOM_TO_TOP:
					case FlxBarFillDirection.VERTICAL_INSIDE_OUT:
					case FlxBarFillDirection.VERTICAL_OUTSIDE_IN:
						_fillHorizontal = false;
						break;
				}

			}
		}
		#endregion

		#region Private fields
		private bool _fillHorizontal;

		/**
		 * FlxSprite which is used for rendering front graphics of bar (showing value) in tile render mode.
		 */
		//var _frontFrame:FlxFrame;

		private RectangleF _filledFlxRect;

		private Texture2D _emptyBar;
		private Rectangle _emptyBarRect;

		private Texture2D _filledBar;

		private Vector2 _zeroOffset;

		private Rectangle _filledBarRect;
		private Vector2 _filledBarPoint;

		private int _maxPercent = 100;
		#endregion

		#region Constructor
		/// <summary>
		/// Create a new FlxBar Object
		/// </summary>
		/// <param name="x">The x coordinate location of the resulting bar (in world pixels)</param>
		/// <param name="y">The y coordinate location of the resulting bar (in world pixels)</param>
		/// <param name="direction">The fill direction, LEFT_TO_RIGHT by default</param>
		/// <param name="width">The width of the bar in pixels</param>
		/// <param name="height">The height of the bar in pixels</param>
		/// <param name="parentRef">A reference to an object in your game that you wish the bar to track</param>
		/// <param name="variable">The variable of the object that is used to determine the bar position. For example if the parent was an FlxSprite this could be "health" to track the health value</param>
		/// <param name="min">The minimum value. I.e. for a progress bar this would be zero (nothing loaded yet)</param>
		/// <param name="max">The maximum value the bar can reach. I.e. for a progress bar this would typically be 100.</param>
		/// <param name="showBorder">Include a 1px border around the bar? (if true it adds +2 to width and height to accommodate it)</param>
		public FlxBar(float x = 0, float y = 0, FlxBarFillDirection? direction = null, int width = 100, int height = 10, FlxObject parentRef = null, string variable = "", float min = 0, float max = 100, bool showBorder = false) : base()
		{
			X = x;
			Y = y;

			direction = (direction == null) ? FlxBarFillDirection.LEFT_TO_RIGHT : direction;

			Width = BarWidth = width;
			Height = BarHeight = height;
			

			_filledBarPoint = new Vector2();
			_filledBarRect = new Rectangle();
			//TODO
			//if (FlxG.renderBlit)
			//{
			//	_zeroOffset = new Vector2();
			//	_emptyBarRect = new Rectangle();
			//	MakeGraphic(width, height, Color.Transparent);
			//}
			//else
			//{
			//	_filledFlxRect = FlxRect.get();
			//}

			if (parentRef != null)
			{
				Parent = parentRef;
				ParentVariable = variable;
			}

			FillDirection = direction.Value;
			CreateFilledBar(Color.FromHex("0xff005100"), Color.FromHex("0xff00F400"), showBorder);
			SetRange(min, max);
		}
		#endregion

		#region Overrides

		public override void Update(GameTime gameTime)
		{
			if (Parent != null)
			{
				if (ParentVariable != null)
				{
					FieldInfo fieldInfo = Parent.GetType().GetField(ParentVariable);
					if ((float?)fieldInfo.GetValue(Parent) != Value)
					{
						UpdateValueFromParent();
					}
				}

				if (!FixedPosition)
				{
					X = Parent.X + PositionOffset.X;
					Y = Parent.Y + PositionOffset.Y;
				}
			}
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			Rectangle filledRect = new Rectangle(_filledBar.Bounds.Location, _filledBar.Bounds.Size);
			filledRect.Width = (int)filledRect.Width * (int)Percent / 100;
			SpriteBatch.Draw(_emptyBar, RenderPosition, _emptyBar.Bounds, Color * Alpha, Rotation, Origin, Scale, Effect, LayerDepth);
			SpriteBatch.Draw(_filledBar, RenderPosition + Vector2.One, filledRect, Color * Alpha, Rotation, Origin, Scale, Effect, LayerDepth);
			//base.Draw(gameTime);
			//TODO
			//if (!FlxG.renderTile)
			//	return;

			//if (alpha == 0)
			//	return;

			//if (percent > 0 && _frontFrame.type != FlxFrameType.EMPTY)
			//{
			//	for (camera in cameras)
			//	{
			//		if (!camera.visible || !camera.exists || !isOnScreen(camera))
			//		{
			//			continue;
			//		}

			//		getScreenPosition(_point, camera).subtractPoint(offset);

			//		_frontFrame.prepareMatrix(_matrix, FlxFrameAngle.ANGLE_0, flipX, flipY);
			//		_matrix.translate(-origin.x, -origin.y);
			//		_matrix.scale(scale.x, scale.y);

			//		// rotate matrix if sprite's graphic isn't prerotated
			//		if (angle != 0)
			//		{
			//			_matrix.rotateWithTrig(_cosAngle, _sinAngle);
			//		}

			//		_point.add(origin.x, origin.y);
			//		if (isPixelPerfectRender(camera))
			//		{
			//			_point.floor();
			//		}

			//		_matrix.translate(_point.x, _point.y);
			//		camera.drawPixels(_frontFrame, _matrix, colorTransform, blend, antialiasing, shader);
			//	}
			//}
		}
		protected override void Dispose(bool disposing)
		{
			//_frontFrame = null;
			_filledFlxRect = null;
			//_emptyBarRect = null;
			//_zeroOffset = null;
			_emptyBar = null;
			_filledBar = null;
			//_filledBarRect = null;
			//_filledBarPoint = null;

			Parent = null;
			//PositionOffset = null;
			EmptyCallback = null;
			FilledCallback = null;
			base.Dispose(disposing);

		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Track the parent FlxSprites x/y coordinates. For example if you wanted your sprite to have a floating health-bar above their head.
		/// If your health bar is 10px tall and you wanted it to appear above your sprite, then set offsetY to be -10
		/// If you wanted it to appear below your sprite, and your sprite was 32px tall, then set offsetY to be 32. Same applies to offsetX.
		/// 
		/// @see		StopTrackingParent
		/// </summary>
		/// <param name="offsetX">The offset on X in relation to the origin x/y of the parent</param>
		/// <param name="offsetY">The offset on Y in relation to the origin x/y of the parent</param>
		public void TrackParent(int offsetX, int offsetY)
		{
			FixedPosition = false;
			PositionOffset = new Vector2(offsetX, offsetY);
			FieldInfo fieldInfo = Parent.GetType().GetField("ScrollFactor");
			if (fieldInfo != null)
			{
				Vector2 parentScrollFactor = (Vector2)fieldInfo.GetValue(Parent);
				ScrollFactor.X = parentScrollFactor.X;
				ScrollFactor.Y = parentScrollFactor.Y;
			}
		}

		/// <summary>
		/// Sets a parent for this FlxBar. Instantly replaces any previously set parent and refreshes the bar.
		/// </summary>
		/// <param name="parentRef">A reference to an object in your game that you wish the bar to track</param>
		/// <param name="variable">The variable of the object that is used to determine the bar position. For example if the parent was an FlxSprite this could be "health" to track the health value</param>
		/// <param name="track">If you wish the FlxBar to track the x/y coordinates of parent set to true (default false)</param>
		/// <param name="offsetX">The offset on X in relation to the origin x/y of the parent</param>
		/// <param name="offsetY">The offset on Y in relation to the origin x/y of the parent</param>
		public void SetParent(FlxObject parentRef, string variable, bool track = false, int offsetX = 0, int offsetY = 0)
		{
			Parent = parentRef;
			ParentVariable = variable;

			if (track)
			{
				TrackParent(offsetX, offsetY);
			}

			UpdateValueFromParent();
		}

		/// <summary>
		/// Tells the health bar to stop following the parent sprite. The given posX and posY values are where it will remain on-screen.
		/// </summary>
		/// <param name="posX">X coordinate of the health bar now it's no longer tracking the parent sprite</param>
		/// <param name="posY">Y coordinate of the health bar now it's no longer tracking the parent sprite</param>
		public void StopTrackingParent(int posX, int posY)
		{
			FixedPosition = true;
			X = posX;
			Y = posY;
		}
		/// <summary>
		/// Sets callbacks which will be triggered when the value of this FlxBar reaches min or max.
		/// Functions will only be called once and not again until the value changes.
		/// Optionally the FlxBar can be killed if it reaches min, but if will fire the empty callback first (if set)
		/// </summary>
		/// <param name="onEmpty">The function that is called if the value of this FlxBar reaches min</param>
		/// <param name="onFilled">The function that is called if the value of this FlxBar reaches max</param>
		/// <param name="killOnEmpty">If set it will call FlxBar.kill() if the value reaches min</param>
		public void SetCallbacks(Action onEmpty, Action onFilled, bool killOnEmpty = false)
		{
			EmptyCallback = (onEmpty != null) ? onEmpty : EmptyCallback;
			FilledCallback = (onFilled != null) ? onFilled : FilledCallback;
			KillOnEmpty = killOnEmpty;
		}

		/// <summary>
		/// Set the minimum and maximum allowed values for the FlxBar
		/// </summary>
		/// <param name="min">The minimum value. I.e. for a progress bar this would be zero (nothing loaded yet)</param>
		/// <param name="max">The maximum value the bar can reach. I.e. for a progress bar this would typically be 100.</param>
		public void SetRange(float min, float max)
		{
			if (max <= min)
			{
				throw new InvalidOperationException("FlxBar: max cannot be less than or equal to min");
			}

			Min = min;
			Max = max;
			Range = max - min;
			Pct = Range / _maxPercent;

			PxPerPercent = (_fillHorizontal) ? (BarWidth / _maxPercent) : (BarHeight / _maxPercent);

			if (Value.HasValue)
			{
				Value = Math.Max(min, Math.Min(Value.Value, max));
			}
			else
			{
				Value = min;
			}
		}


		/// <summary>
		/// Updates health bar view according its current value.
		/// Called when the health bar detects a change in the health of the parent.
		/// </summary>
		public void UpdateBar()
		{
			UpdateEmptyBar();
			UpdateFilledBar();
		}
		/// <summary>
		/// Stamps health bar background on its pixels
		/// </summary>
		public void UpdateEmptyBar()
		{
			//if (FlxG.renderBlit)
			//{
			//	pixels.copyPixels(_emptyBar, _emptyBarRect, _zeroOffset);
			//	dirty = true;
			//}
		}
		/// <summary>
		/// Stamps health bar foreground on its pixels
		/// </summary>
		public void UpdateFilledBar()
		{
			_filledBarRect.Width = BarWidth;
			_filledBarRect.Height = BarHeight;

			float fraction = (Value.Value - Min) / Range;
			float percent = fraction * _maxPercent;
			float maxScale = (_fillHorizontal) ? BarWidth : BarHeight;
			float scaleInterval = maxScale / NumDivisions;
			float interval = (float)Math.Round((fraction * maxScale / scaleInterval) * scaleInterval);

			if (_fillHorizontal)
			{
				_filledBarRect.Width = (int)interval;
			}
			else
			{
				_filledBarRect.Height = (int)interval;
			}

			if (percent > 0)
			{
				switch (FillDirection)
				{
					case FlxBarFillDirection.LEFT_TO_RIGHT:
					case FlxBarFillDirection.TOP_TO_BOTTOM:
						//	Already handled above
						break;

					case FlxBarFillDirection.BOTTOM_TO_TOP:
						_filledBarRect.Y = BarHeight - _filledBarRect.Height;
						_filledBarPoint.Y = BarHeight - _filledBarRect.Height;
						break;
					case FlxBarFillDirection.RIGHT_TO_LEFT:
						_filledBarRect.X = BarWidth - _filledBarRect.Width;
						_filledBarPoint.X = BarWidth - _filledBarRect.Width;
						break;
					case FlxBarFillDirection.HORIZONTAL_INSIDE_OUT:
						_filledBarRect.X = (int)((BarWidth / 2) - (_filledBarRect.Width / 2));
						_filledBarPoint.X = (int)((BarWidth / 2) - (_filledBarRect.Width / 2));
						break;
					case FlxBarFillDirection.HORIZONTAL_OUTSIDE_IN:
						_filledBarRect.Width = (int)(maxScale - interval);
						_filledBarPoint.X = (int)((BarWidth - _filledBarRect.Width) / 2);
						break;
					case FlxBarFillDirection.VERTICAL_INSIDE_OUT:
						_filledBarRect.Y = (int)((BarHeight / 2) - (_filledBarRect.Height / 2));
						_filledBarPoint.Y = (int)((BarHeight / 2) - (_filledBarRect.Height / 2));
						break;
					case FlxBarFillDirection.VERTICAL_OUTSIDE_IN:
						_filledBarRect.Height = (int)(maxScale - interval);
						_filledBarPoint.Y = (int)((BarHeight - _filledBarRect.Height) / 2);
						break;
				}

				//if (FlxG.renderBlit)
				//{
				//	pixels.copyPixels(_filledBar, _filledBarRect, _filledBarPoint, null, null, true);
				//}
				//else
				//{
				//if (FrontFrames != null)
				//	{
				//		_filledFlxRect.copyFromFlash(_filledBarRect).round();
				//		if (Std.int (percent) > 0)
				//		{
				//			_frontFrame = frontFrames.frame.clipTo(_filledFlxRect, _frontFrame);
				//		}
				//	}
				//}
			}

			//if (FlxG.renderBlit)
			//{
			//	dirty = true;
			//}
		}

		#endregion

		#region Graphics Methods

		public FlxBar CreateFilledBar(Color empty, Color fill, bool showBorder = false, Color? border = null)
		{
			border = !border.HasValue ? Color.White : border;
			CreateColoredEmptyBar(empty, showBorder, border);
			CreateColoredFilledBar(fill, showBorder, border);
			return this;
		}
		public FlxBar CreateColoredEmptyBar(Color empty, bool showBorder = false, Color? border = null)
		{
			border = !border.HasValue ? Color.White : border;
			_emptyBar = new Texture2D(GraphicsDevice, BarWidth, BarHeight);
			_emptyBar.FillRect(_emptyBar.Bounds, empty);
			if (showBorder)
			{
				int borderSize = 1;
				_emptyBar.FillRect(new Rectangle(0, 0, _emptyBar.Width, borderSize), border.Value);
				_emptyBar.FillRect(new Rectangle(0, _emptyBar.Height - 1, _emptyBar.Width, borderSize), border.Value);
				_emptyBar.FillRect(new Rectangle(0, 1, borderSize, _emptyBar.Height - 1), border.Value);
				_emptyBar.FillRect(new Rectangle(_emptyBar.Width - 1, 1, borderSize, _emptyBar.Height - 1), border.Value);
			}
			return this;
		}
		public FlxBar CreateColoredFilledBar(Color fill, bool showBorder = false, Color? border = null)
		{
			border = !border.HasValue ? Color.White : border;
			_filledBar = new Texture2D(GraphicsDevice, BarWidth - 2, BarHeight - 2);
			_filledBar.FillRect(_filledBar.Bounds, fill);
			if (showBorder)
			{
				int borderSize = 1;
				_filledBar.FillRect(new Rectangle(0, 0, _filledBar.Width, borderSize), border.Value);
				_filledBar.FillRect(new Rectangle(0, _filledBar.Height - 1, _filledBar.Width, borderSize), border.Value);
				_filledBar.FillRect(new Rectangle(0, 1, borderSize, _filledBar.Height - 1), border.Value);
				_filledBar.FillRect(new Rectangle(_filledBar.Width - 1, 1, borderSize, _filledBar.Height - 1), border.Value);
			}
			return this;
		}

		public FlxBar CreateGradientBar(Color[] empty, Color[] fill, int chunkSize = 1, int rotation = 180, bool showBorder = false,
			Color? border = null)
		{
			border = !border.HasValue ? Color.White : border;
			return this;
		}

		public FlxBar CreateGradientEmptyBar(Color[] empty, int chunkSize = 1, int rotation = 180, bool showBorder = false,
			Color? border = null)
		{
			border = !border.HasValue ? Color.White : border;
			return this;
		}

		public FlxBar CreateGradientFilledBar(Color[] fill, int chunkSize = 1, int rotation = 180, bool showBorder = false,
			Color? border = null)
		{
			border = !border.HasValue ? Color.White : border;
			return this;
		}

		public FlxBar CreateImageBar(Texture2D empty = null, Texture fill = null, Color? emptyBackground = null,
			Color? fillBackground = null)
		{
			emptyBackground = !emptyBackground.HasValue ? Color.Black : emptyBackground;
			fillBackground = !fillBackground.HasValue ? Color.Lime : fillBackground;
			return this;
		}
		public FlxBar CreateImageEmptyBar(Texture2D empty = null, Color? emptyBackground = null)
		{
			emptyBackground = !emptyBackground.HasValue ? Color.Black : emptyBackground;
			return this;
		}

		/// <summary>
		/// Loads given bitmap image for health bar foreground.
		/// </summary>
		/// <param name="fill">Bitmap image used as the foreground (filled part) of the health bar, if null the fillBackground colour is used</param>
		/// <param name="fillBackground">If no foreground (fill) image is given, use this colour value instead. Color.Lime format</param>
		/// <returns></returns>
		public FlxBar createImageFilledBar(Texture2D fill = null, Color? fillBackground = null)
		{
			fillBackground = !fillBackground.HasValue ? Color.Lime : fillBackground;
			return this;
		}
		#endregion

		#region Utils

		void UpdateValueFromParent()
		{
			if (ParentVariable == null)
				return;
			Value = (float?)Parent.GetType().GetField(ParentVariable).GetValue(Parent);
		}
		#endregion
	}
}


public enum FlxBarFillDirection
{
	LEFT_TO_RIGHT,
	RIGHT_TO_LEFT,
	TOP_TO_BOTTOM,
	BOTTOM_TO_TOP,
	HORIZONTAL_INSIDE_OUT,
	HORIZONTAL_OUTSIDE_IN,
	VERTICAL_INSIDE_OUT,
	VERTICAL_OUTSIDE_IN,
}
