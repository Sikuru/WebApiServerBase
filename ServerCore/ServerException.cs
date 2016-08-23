using System;
using System.Runtime.CompilerServices;

namespace ServerCore
{
    public class ServerException : Exception
	{
		public int ResultCode { get; protected set; }

		public ServerException(string message, [CallerLineNumber] int source_line = 0, [CallerMemberName] string calling_method = "", [CallerFilePath] string source_file_path = "")
			: base(string.Format("{0}, {1}@{2}:{3}", message, calling_method, source_file_path, source_line))
		{
		}

		public ServerException(string message, Exception inner_exception, [CallerLineNumber] int source_line = 0, [CallerMemberName] string calling_method = "", [CallerFilePath] string source_file_path = "")
			: base(string.Format("{0}, {1}@{2}:{3}", message, calling_method, source_file_path, source_line), inner_exception)
		{
		}

		public ServerException(int result_code, [CallerLineNumber] int source_line = 0, [CallerMemberName] string calling_method = "", [CallerFilePath] string source_file_path = "")
			: base(string.Format("ResultCode:{0}({1}), {2}@{3}:{4}", result_code.ToString(), (short)result_code, calling_method, source_file_path, source_line))
		{
			this.ResultCode = result_code;
		}

		public ServerException(int result_code, string message, [CallerLineNumber] int source_line = 0, [CallerMemberName] string calling_method = "", [CallerFilePath] string source_file_path = "")
			: base(string.Format("ResultCode:{0}({1}), {2}, {3}@{4}:{5}", result_code.ToString(), (short)result_code, message, calling_method, source_file_path, source_line))
		{
			this.ResultCode = result_code;
		}

		public ServerException(int result_code, string message, Exception inner_exception, [CallerLineNumber] int source_line = 0, [CallerMemberName] string calling_method = "", [CallerFilePath] string source_file_path = "")
			: base(string.Format("ResultCode:{0}({1}), {2}, {3}@{4}:{5}", result_code.ToString(), (short)result_code, message, calling_method, source_file_path, source_line), inner_exception)
		{
			this.ResultCode = result_code;
		}
	}
}
