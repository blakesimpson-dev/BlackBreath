namespace Whetstone.Random
{
   public static class RSingleton
   {
      public static readonly DotNetRandom DefaultRandom = new DotNetRandom();
   }
}
