namespace NPlatform.Domains
{
    /// <summary>
    /// Dapper ORM ����ӳ�����
    /// </summary>
    public class ORMIgnored : System.Attribute
    {
        /// <summary>
        /// ����Id
        /// </summary>
        public override object TypeId
        {
            get
            {
                return "NPlatformEntityORMIgnored";
            }
        }
    }
}