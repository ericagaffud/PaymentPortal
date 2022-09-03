/* using System;   
namespace Fibonacci{  
    class Program{  
        static int Fibonacci(int n){ 
            int first = 0, second = 1, result = 0;  
            if (n == 0){
                return 0; 
            }
            if (n == 1){
                return 1;
            } 
   
            for (int i = 2; i <= n; i++){  
                result = first + second;  
                first = second;  
                second = result;
            }
            return result;  
        }
        static void Main(string[] args){  
            Console.Write("Enter Number of Fibonacci: ");  
            int length = Convert.ToInt32(Console.ReadLine());  
            for (int i = 0; i < length; i++){
                Console.Write("{0} ", Fibonacci(i));
            }  
            Console.Write("\n\nThe " + length + "th number is " + Fibonacci(length-1));
            Console.ReadKey();  
        }  
    }  
} */