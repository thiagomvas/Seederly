using Bogus.DataSets;

namespace Seederly.Core;

public static class Extensions{
    public static string PasswordCustom(this Internet internet){
      
        var r = internet.Random;
      
        var number = r.Replace("#");  // length 1
        var letter = r.Replace("?");  // length 2
        var lowerLetter = letter.ToLower(); //length 3
        var symbol = r.Char((char)33, (char)47); //length 4 - ascii range 33 to 47

        var padding = r.String2(r.Number(4, 8)); //length 6 - 10
      
        return new string( r.Shuffle(number + letter + lowerLetter + symbol + padding).ToArray() );
    }
}