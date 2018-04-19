using System;
namespace TagLib.IFD.Entries
{
	public struct Rational:
	IFormattable
	{
#region Private Fields
		private uint numerator;
		private uint denominator;
#endregion
#region Constructor
		public Rational(uint numerator,uint denominator)
		{
			if(denominator==0)
			{
				throw new ArgumentException("denominator");
			}
			this.numerator=numerator;
			this.denominator=denominator;
		}
#endregion
#region Public Methods
		public Rational Reduce()
		{
			uint gcd=Denominator;
			uint b=Numerator;
			while(b!=0)
			{
				uint tmp=gcd%b;
				gcd=b;
				b=tmp;
			}
			return new Rational(Numerator/gcd,Denominator/gcd);
		}
		public string ToString(string format,IFormatProvider provider)
		{
			Rational reduced=Reduce();
			return String.Format("{0}/{1}",reduced.Numerator,reduced.Denominator);
		}
		public override string ToString()
		{
			return String.Format("{0}",this);
		}
#endregion
#region Public Properties
		public uint Numerator
		{
			get
			{
				return numerator;
			}
			set
			{
				numerator=value;
			}
		}
		public uint Denominator
		{
			get
			{
				return denominator;
			}
			set
			{
				if(value==0)
				{
					throw new ArgumentException("denominator");
				}
				denominator=value;
			}
		}
#endregion
#region Public Static Methods
		public static implicit operator double(Rational rat)
		{
			return(double)rat.Numerator/(double)rat.Denominator;
		}
#endregion
	}
}
