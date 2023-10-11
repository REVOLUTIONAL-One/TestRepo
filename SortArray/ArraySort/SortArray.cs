using SortArray.ArraySort.Enumeration;

namespace SortArray
{
    /// <summary>
    /// Represents an array sorting algorithm class.
    /// </summary>
    public class SortArray
    {
        #region Public Method

        /// <summary>
        /// Sorts a two dimensional array using merge sort.
        /// </summary>
        /// <param name="numbersArray">User inputed two dimensional array of type double.</param>
        /// <param name="leftIndex">Starting index of the array from where the sorting need to begin.</param>
        /// <param name="rightIndex">Ending index of the array at where the sorting need to stop.</param>
        /// <param name="sortOperation">The operation which needs to be performed on the array.</param>
        /// <param name="sortColumn">Sort the array according which column.</param>
        /// <param name="errorMessage">If Validation failed then error message will store the error.</param>
        /// <returns>Sorted two dimensional double array.</returns>
        public (bool sortingResult, double[,] sortedArray) ArraySort(double[,] numbersArray, ulong leftIndex, ulong rightIndex, SortOperation sortOperation, ulong sortColumn, out string? errorMessage)
        {
            try
            {
                // Validates the left and right indexes are valid or not.
                if (leftIndex > Convert.ToUInt64(numbersArray.GetLength(0) - 1) ||
                    rightIndex > Convert.ToUInt64(numbersArray.GetLength(0) - 1))
                {
                    errorMessage = "Indexes are out of bound, indexes should be within the range of the array.";
                    return (false, numbersArray);
                }

                if (leftIndex > rightIndex)
                {
                    errorMessage = "Invalid left index and right index value, left index should be lesser than or equal to right index.";
                    return (false, numbersArray);
                }

                // Validates sort column is valid or not.
                if (sortColumn > Convert.ToUInt64(numbersArray.GetLength(1)) || sortColumn < 1)
                {
                    errorMessage = $"Invalid sort column value, sort column value should be between 1 to {numbersArray.GetLength(1)}";
                    return (false, numbersArray);
                }

                errorMessage = null;

                // Checks if the subarray length is less than two or not.
                if (rightIndex > leftIndex)
                {
                    // Calculates middle index which lies half way between starting index and ending index of an array.
                    ulong middleIndex = (rightIndex + leftIndex) / 2;

                    // Recursively calls itself until the array is divided into smaller subarray of length two.
                    this.ArraySort(numbersArray, leftIndex, middleIndex, sortOperation, sortColumn, out errorMessage);
                    this.ArraySort(numbersArray, middleIndex + 1, rightIndex, sortOperation, sortColumn, out errorMessage);

                    // Sorts arrays and merges them back into single array.
                    this.MergeSort(numbersArray, leftIndex, middleIndex + 1, rightIndex, sortOperation, sortColumn);
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return (false, numbersArray);
            }

            return (true, numbersArray);
        }

        #endregion

        #region Private Method

        /// <summary>
        /// Performs sorting operation on an arrays and merges them.
        /// </summary>
        /// <param name="numberArray">User inputed two dimensional array of type double.</param>
        /// <param name="leftIndex">Starting index of the array from where the sorting need to begin.</param>
        /// <param name="middleIndex">Middle Index which lies in half way between starting index and ending index of an array.</param>
        /// <param name="rightIndex">Ending index of the array at where the sorting need to stop.</param>
        /// <param name="sortOperation">The operation which needs to be performed on the array.</param>
        /// <param name="sortAccording">Sort the array according which column.</param>
        /// <returns>Sorted two dimensional double array.</returns>
        private double[,] MergeSort(double[,] numberArray,
                                    ulong leftIndex,
                                    ulong middleIndex,
                                    ulong rightIndex,
                                    SortOperation sortOperation,
                                    ulong sortAccording)
        {
            double[,] subArray = new double[numberArray.Length, 2];
            ulong leftEndIndex = middleIndex - 1;
            ulong elementIndex = leftIndex;
            ulong numberOfElements = rightIndex - leftIndex + 1;

            // Checks whether staring index are lesser or equal to ending index.
            while (leftIndex <= leftEndIndex && middleIndex <= rightIndex)
            {
                // Ternary Operator to validate which operation to perform on the array and sort according to which column.
                if (sortAccording.Equals(1) ?
                    !sortOperation.Equals(SortOperation.Ascending) ? numberArray[leftIndex, 0] >= numberArray[middleIndex, 0] :
                    numberArray[leftIndex, 0] <= numberArray[middleIndex, 0] :
                    !sortOperation.Equals(SortOperation.Ascending) ? numberArray[leftIndex, 1] >= numberArray[middleIndex, 1] :
                    numberArray[leftIndex, 1] <= numberArray[middleIndex, 1])
                {
                    // Swaps the elements and stores in sub array.
                    subArray[elementIndex, 0] = numberArray[leftIndex, 0];
                    subArray[elementIndex++, 1] = numberArray[leftIndex++, 1];
                }
                else
                {
                    // Swaps the elements and stores in sub array.
                    subArray[elementIndex, 0] = numberArray[middleIndex, 0];
                    subArray[elementIndex++, 1] = numberArray[middleIndex++, 1];
                }
            }

            // Store the first array in sub array.
            while (leftIndex <= leftEndIndex)
            {
                subArray[elementIndex, 0] = numberArray[leftIndex, 0];
                subArray[elementIndex++, 1] = numberArray[leftIndex++, 1];
            }

            // Merges the second array with first array into sub array.
            while (middleIndex <= rightIndex)
            {
                subArray[elementIndex, 0] = numberArray[middleIndex, 0];
                subArray[elementIndex++, 1] = numberArray[middleIndex++, 1];
            }

            // Overwrites a part of number array with the sorted sub array.
            for (ulong index = 0; index < numberOfElements; index++)
            {
                numberArray[rightIndex, 0] = subArray[rightIndex, 0];
                numberArray[rightIndex, 1] = subArray[rightIndex, 1];
                rightIndex--;
            }

            return numberArray;
        }

        #endregion
    }
}
