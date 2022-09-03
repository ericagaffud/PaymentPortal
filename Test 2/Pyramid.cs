using System;
namespace Pyramid{
    class Program{
        static void Main(string[] args){
            int i, j, length; 
            
            Console.Write("Enter Number of Rows:");
            length = Convert.ToInt32(Console.ReadLine());

            for(i = 0; i < length; i++){
                for(j = 0; j <= length-i; j++)
                Console.Write(" ");
                for(j = 1; j <= 2*i-1; j++)
                    Console.Write("*");
                Console.Write("\n");
            }
            
        }
    }
}