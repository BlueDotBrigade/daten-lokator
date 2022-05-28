namespace BlueDotBrigade.DatenLokator.TestsTools
{
    public static class StringExtensions
    {
        /// <summary>
        /// Identifies the position where the prefixes no longer match.
        /// </summary>
        /// <returns>The zero-based index position of the last matching value, or -1 if the strings do not have a matching prefix.</returns>
        public static int LastIndexOfPrefix(this string instance, string value)
        {
            const int NotMatchingCharacters = -1;

            if (string.IsNullOrWhiteSpace(instance) || string.IsNullOrWhiteSpace(value))
            {
                return NotMatchingCharacters;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return NotMatchingCharacters;
                }
                else
                {
                    int indexOf = NotMatchingCharacters;
                    int count = instance.Length < value.Length ? instance.Length : value.Length;

                    for (int i = 0; i < count; i++)
                    {
                        if (instance[i] != value[i])
                        {
                            break;
                        }

                        indexOf = i;
                    }

                    return indexOf;
                }
            }
        }
    }
}