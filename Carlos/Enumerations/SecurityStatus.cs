namespace Carlos.Enumerations
{
   /// <summary>
   /// 这是一个列举所有加密状态的枚举，包含但并不限于文本加密，文件加密，信道加密和数据流加密的状态等等。
   /// </summary>
   public enum SecurityStatus : int
   {
      /// <summary>
      /// 安全状态或者加密状态为已加密。
      /// </summary>
      Encrypted = 0x0000,
      /// <summary>
      /// 安全状态或者加密状态为未加密。
      /// </summary>
      Unencrypted = 0x0001,
      /// <summary>
      /// 无法判断安全状态或者加密状态。
      /// </summary>
      CannotJudge = 0xffff
   }
}
