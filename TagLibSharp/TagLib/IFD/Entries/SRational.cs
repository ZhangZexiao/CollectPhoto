using System;
namespace TagLib.IFD.Entries
{
	public struct SRational:
	IFormattable
	{
#region Private Fields
		private int numerator;
		private int denominator;
#endregion
#region Constructor
		public SRational(int numerator,int denominator)
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
		public SRational Reduce()
		{
			int den_sign=Math.Sign(Denominator);
			int gcd=Math.Abs(Denominator);
			int b=Math.Abs(Numerator);
			while(b!=0)
			{
				int tmp=gcd%b;
				gcd=b;
				b=tmp;
			}
			return new SRational(den_sign*(Numerator/gcd),Math.Abs(Denominator)/gcd);
		}
		public string ToString(string format,IFormatProvider provider)
		{
			SRational reduced=Reduce();
			return String.Format("{0}/{1}",reduced.Numerator,reduced.Denominator);
		}
		public override string ToString()
		{
			return String.Format("{0}",this);
		}
#endregion
#region Public Properties
		public int Numerator
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
		public int Denominator
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
		public static implicit operator double(SRational rat)
		{
			return(double)rat.Numerator/(double)rat.Denominator;
		}
#endregion
	}
}
