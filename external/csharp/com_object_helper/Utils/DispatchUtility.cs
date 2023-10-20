using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Security.Permissions;

namespace Utils {
	public static class DispatchUtility {

		private const int S_OK = 0;
		//From WinError.h
		private const int LOCALE_SYSTEM_DEFAULT = 2 << 10;
		//From WinNT.h == 2048 == 0x800

		// attempt to cast
		public static bool ImplementsIDispatch(object obj) {
			bool result = obj is IDispatchInfo;
			return result;
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public static Type GetType(object obj, bool throwIfNotFound) {
			RequireReference(obj, "obj");
			Type result = GetType((IDispatchInfo)obj, throwIfNotFound);
			return result;
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public static bool TryGetDispId(object obj, string name, out int dispId) {
			RequireReference(obj, "obj");
			bool result = TryGetDispId((IDispatchInfo)obj, name, out dispId);
			return result;
		}

		public static object Invoke(object obj, int dispId, object[] args) {
			string memberName = "[DispId=" + dispId + "]";
			object result = Invoke(obj, memberName, args);
			return result;
		}

		public static object Invoke(object obj, string memberName, object[] args) {
			RequireReference(obj, "obj");
			Type type = obj.GetType();
			object result = type.InvokeMember(memberName, BindingFlags.InvokeMethod | BindingFlags.GetProperty,
				                null, obj, args, null);
			return result;
		}

		private static void RequireReference<T>(T value, string name) where T : class {
			if (value == null) {
				throw new ArgumentNullException(name);
			}
		}

		private static Type GetType(IDispatchInfo dispatch, bool throwIfNotFound) {
			RequireReference(dispatch, "dispatch");

			Type result = null;
			int typeInfoCount;
			int hr = dispatch.GetTypeInfoCount(out typeInfoCount);
			if (hr == S_OK && typeInfoCount > 0) {
				// Type info isn't usually culture-aware for IDispatch, so we might as well pass
				// the default locale instead of looking up the current thread's LCID each time
				// (via CultureInfo.CurrentCulture.LCID).
				dispatch.GetTypeInfo(0, LOCALE_SYSTEM_DEFAULT, out result);
			}

			if (result == null && throwIfNotFound) {
				// If the GetTypeInfoCount called failed, throw an exception for that.
				Marshal.ThrowExceptionForHR(hr);

				// Otherwise, throw the same exception that Type.GetType would throw.
				throw new TypeLoadException();
			}

			return result;
		}

		private static bool TryGetDispId(IDispatchInfo dispatch, string name, out int dispId) {
			RequireReference(dispatch, "dispatch");
			RequireReference(name, "name");

			bool result = false;

			// Members names aren't usually culture-aware for IDispatch, so we might as well
			// pass the default locale instead of looking up the current thread's LCID each time
			// (via CultureInfo.CurrentCulture.LCID).
			Guid iidNull = Guid.Empty;
			int hr = dispatch.GetDispId(ref iidNull, ref name, 1, LOCALE_SYSTEM_DEFAULT, out dispId);

			const int DISP_E_UNKNOWNNAME = unchecked((int)0x80020006); //From WinError.h
			const int DISPID_UNKNOWN = -1; //From OAIdl.idl
			if (hr == S_OK) {
				result = true;
			} else if (hr == DISP_E_UNKNOWNNAME && dispId == DISPID_UNKNOWN) {
				// This is the only supported "error" case because it means IDispatch
				// is saying it doesn't know the member we asked about.
				result = false;
			} else {
				// The other documented result codes are all errors.
				Marshal.ThrowExceptionForHR(hr);
			}

			return result;
		}

		[ComImport]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[Guid("00020400-0000-0000-C000-000000000046")]
		private interface IDispatchInfo {
			// http://msdn.microsoft.com/en-us/library/da876d53-cb8a-465c-a43e-c0eb272e2a12(VS.85)
			[PreserveSig]
			int GetTypeInfoCount(out int typeInfoCount);
			// https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nf-oaidl-idispatch-gettypeinfo
			void GetTypeInfo(int typeInfoIndex, int lcid, [MarshalAs(UnmanagedType.CustomMarshaler,
				MarshalTypeRef = typeof(System.Runtime.InteropServices.CustomMarshalers.TypeToTypeInfoMarshaler))] out Type typeInfo);

			[PreserveSig]
			int GetDispId(ref Guid riid, ref string name, int nameCount, int lcid, out int dispId);

			// NOTE: The real IDispatch also has an Invoke method next, but we don't need it.
			// We can invoke methods using .NET's Type.InvokeMember method with the special
			// [DISPID=n] syntax for member "names", or we can get a .NET Type using GetTypeInfo
			// and invoke methods on that through reflection.
			// Type.InvokeMember: http://msdn.microsoft.com/en-us/library/de3dhzwy.aspx
		}

	}
}
