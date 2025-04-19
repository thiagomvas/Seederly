using Bogus.DataSets;

namespace Seederly.Core;

public static class Extensions
{
    /// <summary>
    /// Generates a random password that is 6 - 10 characters long, with at least one lowercase letter, uppercase letter, number and special character.
    /// </summary>
    /// <param name="internet">
    /// The <see cref="Internet"/> instance to use for generating the password.
    /// </param>
    /// <returns>
    /// A random password string.
    /// </returns>
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