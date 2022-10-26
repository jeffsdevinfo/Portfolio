using System.Collections.Generic;

public class Utility
{//TODO: add more todos
    public static float[] TwoDToOneDArray(float[,] input)
    {
        // Step 1: get total size of 2D array, and allocate 1D array.
        int size = input.Length;
        float[] result = new float[size];

        // Step 2: copy 2D array elements into a 1D array.
        int write = 0;
        for (int i = 0; i <= input.GetUpperBound(0); i++)
        {
            for (int z = 0; z <= input.GetUpperBound(1); z++)
            {
                result[write++] = input[i, z];
            }
        }
        // Step 3: return the new array.
        return result;
    }

    public static float[,] OneDToTwoDArray(float[] input, int rows, int columns)
    {
        float[,] twoDimensionalArray = new float[rows, columns];
        int index = 0;
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                twoDimensionalArray[x, y] = input[index];
                index++;
            }
        }
        return twoDimensionalArray;
    }

    public static float[] ListFloatTo1DArray(List<float> inputArray)
    {
        return inputArray.ToArray();
    }
}
