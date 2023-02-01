namespace GGJ.Levels
{
    public class RoomConnectionData
    {
        public int roomIndex;
        public int depth;
        public int availableConnections;

        public RoomConnectionData[] subRooms;

        public override string ToString()
        {
            string Tabs(int count)
            {
                var tabs = string.Empty;
                for (int i = 0; i < count; i++)
                {
                    tabs += "   ";
                }

                return $"{(count > 0 ? "|" : "")}{tabs}";
                return "|" + tabs + "└";
            }

            //------------------------------------------------//
            var outString = string.Empty;

            outString += $"{Tabs(depth)}{(depth > 0 ? "└" : "")}[{roomIndex}] : {depth}";
            if (subRooms == null || subRooms.Length == 0)
                return outString;
            for (int i = 0; i < subRooms.Length; i++)
            {
                if(subRooms[i] == null)
                    continue;
                
                outString += $"\n{Tabs(depth)}{subRooms[i]}";
            }

            return outString;
        }
    }
}