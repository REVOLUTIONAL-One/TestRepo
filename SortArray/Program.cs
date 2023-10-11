using System.Management;
using System.Numerics;
using System.Runtime.InteropServices;
using SortArray.ArraySort.Enumeration;

namespace SortArray
{
    /// <summary>
    /// Represent a program for sorting a two dimensional array.
    /// </summary>
    public class Program
    {
        #region Public Method

        /// <summary>
        /// Program to sort an two dimension array.
        /// </summary>
        public static void Main()
        {
            Console.WriteLine(new string('-', 39));
            Console.WriteLine("2D ARRAY SORTER:");
            Console.WriteLine(new string('-', 39));

            // Gets the two dimensional array and other parameters from the user.
            bool getArrayResult = GetTwoDimArray(out double[,] twoDimArray,
                                         out ulong numberOfElements,
                                         out ulong columnNumberToSort,
                                         out SortOperation sortOperationToPerform);

            // Gets the two dimensional array from the user as well as various other parameters.
            if (!getArrayResult)
            {
                Console.WriteLine("An Error occured when inputing user's array input.");
                return;
            }

            do
            {
                SortArray sortArray = new ();

                // Sorts the array using merge sort.
                var sortingStatus = sortArray.ArraySort(twoDimArray,
                                         0,
                                         numberOfElements - 1,
                                         sortOperationToPerform,
                                         columnNumberToSort,
                                         out string? errorMessage);

                // Validates if the array sorting was successful or not.
                if (!sortingStatus.sortingResult)
                {
                    Console.WriteLine($"Error occured when sorting the array,\n{errorMessage}");
                    return;
                }

                // Displays the two dimensional array in tabular form.
                OutputSortedArray(sortingStatus.sortedArray, numberOfElements);

                byte userChoice;

                do
                {
                    // Checks if user want to re-sort or not.
                    do
                    {
                        Console.Write(string.Concat(
                            "\nDo you want to:\n",
                            new string('-', 39),
                            "\n1.Re-sort the array.\n2.Edit any elememt\n3.Exit\nChoose an option: "));

                        if (!byte.TryParse(Console.ReadLine(), out userChoice) || userChoice > 3 || userChoice < 1)
                        {
                            Console.WriteLine("\nInvalid input, input should be between 1 to 3.");
                            Console.WriteLine(new string('-', 39));

                            continue;
                        }

                        Console.WriteLine(new string('-', 39));
                        break;
                    }
                    while (true);

                    if (userChoice == 3)
                    {
                        Console.Write("\nPress any key to close this window . . .");
                        Console.ReadKey();
                        return;
                    }

                    if (userChoice == 2)
                    {
                        // Edits and updates the two dimensional array.
                        twoDimArray = EditTwoDimArray(twoDimArray);

                        // Updates the number of elements of the array.
                        numberOfElements = Convert.ToUInt64(twoDimArray.GetLength(0));

                        continue;
                    }

                    if (userChoice == 1)
                    {
                        break;
                    }
                }
                while (true);

                // Updates the sorting column of the array.
                columnNumberToSort = ColumnNumberToSort();

                // Updates the sorting operation needs to be performed on the array.
                sortOperationToPerform = SortOperationToPerform();
            }
            while (true);
        }

        #endregion

        #region Private Method

        private static void OutputSortedArray(double[,] twoDimArray, ulong numberOfElements)
        {
            Console.WriteLine(string.Concat("\n\nSorted Array:\n", new string('-', 33)));
            Console.WriteLine(string.Format("|{0,10}\t|{1,10}\t|", 'X', 'Y'));

            // Displays array in tabular form.
            for (ulong i = 0; i < numberOfElements; i++)
            {
                Console.WriteLine(new string('-', 33));
                Console.WriteLine(
                    string.Format("|{0,10}\t|{1,10}\t|", twoDimArray[i, 0], twoDimArray[i, 1]));
            }

            Console.WriteLine(string.Concat(new string('-', 33), "\n"));
        }

        /// <summary>
        /// Gets the element <see langword="values"/> and stores in a <c><see langword="two"/></c> dimensional
        /// <see cref="Array"/>.
        /// </summary>
        /// <returns>A <see cref="Tuple"/> consisting of a <c><see langword="two"/></c> dimensional <see cref="Array"/>
        /// and other <see langword="parameters"/>.</returns>
        private static bool GetTwoDimArray(out double[,] twoDimArray, out ulong numberOfElements, out ulong columnNumberToSort, out SortOperation sortOperationToPerform)
        {
            try
            {
                // Gets the total number of element in the two dimensional array.
                do
                {
                    // Gets the elements of array as string.
                    Console.Write("Enter the total number of elements: ");

                    // Parses and check if the number of elements value is valid or not.
                    if (!ulong.TryParse(Console.ReadLine(), out numberOfElements) || numberOfElements > 1073741824)
                    {
                        Console.WriteLine(string.Concat(
                            "\n",
                            new string('-', 39),
                            "\nInvalid input, number of elements should be a\n",
                            "positive integer value and should be less than\n",
                            @"or equal to ""1073741824"".",
                            "\n",
                            new string('-', 39)));

                        continue;
                    }

                    // Checks if there is enough memory in the system.
                    if (!IsMemorySufficient(numberOfElements, out string errorMessage))
                    {
                        Console.Write("\n");
                        Console.WriteLine(new string('-', 39));
                        Console.WriteLine(errorMessage);
                        Console.WriteLine(new string('-', 39));

                        continue;
                    }

                    break;
                }
                while (true);

                // Created a two dimensional array of size number of elements.
                twoDimArray = new double[numberOfElements, 2];

                for (ulong index = 1; index <= numberOfElements; index++)
                {
                    // Gets the element values from the user.
                    twoDimArray = InputElementValues(twoDimArray, index);
                }

                string editElement;

                Console.Write("\n");

                // Check if user want to edit any element in the array.
                do
                {
                    Console.Write("Do you want to edit any element [Y/N]: ");
                    editElement = Console.ReadLine().ToUpper();

                    // Check if the input is valid or not.
                    if (editElement != "Y" && editElement != "N")
                    {
                        Console.WriteLine("\nInvalid input, either enter Y or N.");
                        Console.WriteLine(new string('-', 39));
                    }
                }
                while (editElement != "Y" && editElement != "N");

                if (editElement == "Y")
                {
                    // Gets the updated two dimensional array.
                    twoDimArray = EditTwoDimArray(twoDimArray);
                }

                // Gets the column number to sort.
                columnNumberToSort = ColumnNumberToSort();

                // Gets the sorting operation to perform.
                sortOperationToPerform = SortOperationToPerform();

                return true;
            }
            catch (Exception ex)
            {
                // Checks if exception is occured due to insufficient memory.
                if (ex is InsufficientMemoryException)
                {
                    Console.WriteLine("\nMemory is insufficient to run the program.");

                    twoDimArray = new double[0, 0];
                    numberOfElements = 0;
                    columnNumberToSort = 0;
                    sortOperationToPerform = SortOperation.Ascending;

                    return false;
                }

                Console.WriteLine($"Error occured while inputing user inputs:\n{ex.Message}");

                twoDimArray = new double[0, 0];
                numberOfElements = 0;
                columnNumberToSort = 0;
                sortOperationToPerform = SortOperation.Ascending;

                return false;
            }
        }

        /// <summary>
        /// Verifies if the system has sufficient memory.
        /// </summary>
        /// <param name="numberOfElements">The size of the array to be creater.</param>
        /// <param name="errorMessage">Sends back error message to the caller.</param>
        /// <returns>True: If memory is sufficient, False: If memory is insufficient.</returns>
        private static bool IsMemorySufficient(ulong numberOfElements, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Construct a query to retrieve information about the operating system.
            ObjectQuery winQuery = new ("SELECT * FROM Win32_OperatingSystem");

            // Create a searcher to execute the query
            ManagementObjectSearcher searcher = new (winQuery);
            ManagementObjectCollection asf = searcher.Get(); // Get the collection of management objects.

            // Initialize variables to store physical memory information.
            ulong totalPhysicalMemory = 0;
            ulong totalFreeMemory = 0;

            // Iterate through the management objects in the collection.
            foreach (ManagementObject item in asf)
            {
                // Retrieve and convert total physical memory and free physical memory.
                totalPhysicalMemory = Convert.ToUInt64(item["TotalVisibleMemorySize"].ToString()) * 1024;
                totalFreeMemory = Convert.ToUInt64(item["FreePhysicalMemory"].ToString()) * 1024;
            }

            // Calculate the required memory based on the number of elements.
            BigInteger requiredMemory = BigInteger.Parse(Marshal.SizeOf(typeof(double)).ToString()) * 2 * numberOfElements;

            // Check if the required memory exceeds available free memory.
            if (requiredMemory > (totalFreeMemory - (totalFreeMemory * 5 / 100)))
            {
                // Calculate the recommended maximum number of elements.
                BigInteger maxNumberOfElements = totalFreeMemory / (BigInteger.Parse(Marshal.SizeOf(typeof(double)).ToString()) * 2);

                errorMessage = string.Format(string.Concat(
                    "The Memory it would require to create\n",
                    @"An array of size ""{0}"" will exceed the",
                    "\ncapacity of main memory.\n",
                    "Total Physical RAM Memory: {1:f2} GB\n",
                    "Total Free RAM Memory: {2:F2} GB\n",
                    "Recommended max number of element: {3}"),
                    numberOfElements,
                    totalPhysicalMemory / 1024 / 1024 / Convert.ToDouble(1024),
                    totalFreeMemory / 1024 / 1024 / Convert.ToDouble(1024),
                    maxNumberOfElements - (maxNumberOfElements * 10 / 100));

                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets an elements <see langword="X"/> and <see langword="Y"/> <see langword="values"/> from the user.
        /// </summary>
        /// <param name="twoDimArray">Two dimensional array in which the values are going to be stored.</param>
        /// <param name="elementNumber">The element number at which the values are going to be stored.</param>
        /// <returns>A <c><see langword="two"/></c> dimensional <see cref="double"/> <see cref="Array"/>.</returns>
        private static double[,] InputElementValues(double[,] twoDimArray, ulong elementNumber)
        {
            Console.WriteLine($"\nEnter X and Y values of element {elementNumber}:");
            Console.WriteLine(new string('-', 39));

            do
            {
                Console.Write("Enter the X value: ");

                // Parses and checks if user input x value is valid or not.
                if (!double.TryParse(Console.ReadLine(), out twoDimArray[elementNumber - 1, 0]))
                {
                    Console.WriteLine("\nInvalid X input value, enter a valid integer or a decimal value.");
                    Console.WriteLine(new string('-', 39));
                    continue;
                }

                break;
            }
            while (true);

            do
            {
                Console.Write("Enter the Y value: ");

                // Parses and checks if user input x value is valid or not.
                if (!double.TryParse(Console.ReadLine(), out twoDimArray[elementNumber - 1, 1]))
                {
                    Console.WriteLine("\nInvalid Y input value, enter a valid integer or a decimal value.");
                    Console.WriteLine(new string('-', 39));
                    continue;
                }

                break;
            }
            while (true);

            Console.WriteLine(new string('-', 39));

            return twoDimArray;
        }

        /// <summary>
        /// Edits any <see langword="element"/> within the <c><see langword="two"/></c> dimensional <see cref="Array"/>.
        /// </summary>
        /// <param name="twoDimArray">The <c><see langword="two"/></c> dimensional <see cref="Array"/> to be edited.</param>
        /// <returns>The edited <c><see langword="two"/></c> dimensional <see cref="Array"/>.</returns>
        private static double[,] EditTwoDimArray(double[,] twoDimArray)
        {
            do
            {
                ulong elementNumber;
                ulong numberOfElements = 0;

                try
                {
                    numberOfElements = Convert.ToUInt64(twoDimArray.Length / 2);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occured while parsing array's length to number of elements.\n{ex.Message}");
                    return twoDimArray;
                }

                Console.Write("\n");

                // Input the element number to be edited.
                do
                {
                    // Gets the elements of array as string.
                    Console.Write("Enter the element number be to edited: ");

                    // Parses and check if the element number is valid or not.
                    if (!ulong.TryParse(Console.ReadLine(), out elementNumber) || elementNumber > numberOfElements || elementNumber < 1)
                    {
                        Console.WriteLine(string.Concat(
                            "\n",
                            new string('-', 41),
                            "\nInvalid input, element number should be a\n",
                            "positive integer value and should be less\n",
                            @$"than or equal to ""{numberOfElements}"" and greater than ""0"".",
                            "\n",
                            new string('-', 41)));

                        continue;
                    }

                    break;
                }
                while (true);

                // Gets the element's X and Y value.
                twoDimArray = InputElementValues(twoDimArray, elementNumber);

                string editElement;

                // Checks if user wants to edit any element.
                do
                {
                    Console.Write("\nDo you want to edit any element [Y/N]: ");
                    editElement = Console.ReadLine().ToUpper();

                    // Checks if the user input is valid or not.
                    if (editElement != "Y" && editElement != "N")
                    {
                        Console.WriteLine("\n\nInvalid input, either enter Y or N.");
                        Console.WriteLine(new string('-', 39));
                    }
                }
                while (editElement != "Y" && editElement != "N");

                if (editElement == "N")
                {
                    break;
                }
            }
            while (true);

            // Returns the edited two dimensional array.
            return twoDimArray;
        }

        /// <summary>
        /// Inputs the <see langword="column"/> which needs to be <see langword="sorted"/>.
        /// </summary>
        /// <returns>The <see langword="column"/> number to be <see langword="sorted"/>.</returns>
        private static ulong ColumnNumberToSort()
        {
            ulong sortColumnNumber;

            do
            {
                Console.Write(string.Concat(
                    "\nEnter sort according to column [X / Y]:\n",
                    new string('-', 39),
                    "\n1.X\n2.Y\nChoose an option: "));

                // Parses and checks if the column number is valid or not.
                if (!ulong.TryParse(Console.ReadLine(), out sortColumnNumber) || sortColumnNumber > 2 || sortColumnNumber < 1)
                {
                    Console.WriteLine("\nInvalid choice, choice should be either 1 or 2.");
                    continue;
                }

                Console.WriteLine(new string('-', 39));

                break;
            }
            while (true);

            return sortColumnNumber;
        }

        /// <summary>
        /// Inputs the <see langword="sorting"/> operation to be performed.
        /// </summary>
        /// <returns>The <see langword="sorting"/> operation which will be performed.</returns>
        private static SortOperation SortOperationToPerform()
        {
            SortOperation sortOperation;

            do
            {
                Console.Write(string.Concat(
                    "\nEnter sort order:\n",
                    new string('-', 39),
                    "\n1.Ascending order\n2.Descending order\nChoos an option: "));

                // Parses and check whether the user input is a valid operation.
                if (!Enum.TryParse(Console.ReadLine(), out sortOperation) || !Enum.IsDefined(typeof(SortOperation), sortOperation))
                {
                    Console.WriteLine("\nInvalid input, choice should be either 1 or 2.");
                    continue;
                }

                Console.WriteLine(new string('-', 39));

                break;
            }
            while (true);

            return sortOperation;
        }

        #endregion
    }
}