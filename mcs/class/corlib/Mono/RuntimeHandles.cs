//
// Wrapper handles for Mono Runtime internal structs
//
// Authors:
//   Aleksey Kliger <aleksey@xamarin.com>
//   Rodrigo Kumpera <kumpera@xamarin.com>
//
// Copyright 2016 Dot net foundation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mono {

	internal struct RuntimeClassHandle {
		unsafe RuntimeStructs.MonoClass* value;

		internal unsafe RuntimeClassHandle (RuntimeStructs.MonoClass* value) {
			this.value = value;
		}

		internal unsafe RuntimeClassHandle (IntPtr ptr) {
			this.value = (RuntimeStructs.MonoClass*) ptr;
		}

		internal unsafe RuntimeStructs.MonoClass* Value {
			get { return value; }
		}

		public override bool Equals (object obj)
		{
			if (obj == null || GetType () != obj.GetType ())
				return false;

			unsafe { return value == ((RuntimeClassHandle)obj).Value; }
		}

		public override int GetHashCode ()
		{
			unsafe { return ((IntPtr)value).GetHashCode (); }
		}

		public bool Equals (RuntimeClassHandle handle)
		{
			unsafe { return value == handle.Value; }
		}

		public static bool operator == (RuntimeClassHandle left, Object right)
		{
			return (right != null) && (right is RuntimeClassHandle) && left.Equals ((RuntimeClassHandle)right);
		}

		public static bool operator != (RuntimeClassHandle left, Object right)
		{
			return (right == null) || !(right is RuntimeClassHandle) || !left.Equals ((RuntimeClassHandle)right);
		}

		public static bool operator == (Object left, RuntimeClassHandle right)
		{
			return (left != null) && (left is RuntimeClassHandle) && ((RuntimeClassHandle)left).Equals (right);
		}

		public static bool operator != (Object left, RuntimeClassHandle right)
		{
			return (left == null) || !(left is RuntimeClassHandle) || !((RuntimeClassHandle)left).Equals (right);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe extern static IntPtr GetTypeFromClass (RuntimeStructs.MonoClass *klass);

		internal RuntimeTypeHandle GetTypeHandle ()
		{
			unsafe { return new RuntimeTypeHandle (GetTypeFromClass (value)); }
		}
	}

	internal struct RuntimeRemoteClassHandle {
		unsafe RuntimeStructs.RemoteClass* value;

		internal unsafe RuntimeRemoteClassHandle (RuntimeStructs.RemoteClass* value)
		{
			this.value = value;
		}

		internal RuntimeClassHandle ProxyClass {
			get {
				unsafe {
					return new RuntimeClassHandle (value->proxy_class);
				}
			}
		}
	}

	internal struct RuntimeGenericParamInfoHandle {
		unsafe RuntimeStructs.GenericParamInfo* value;

		internal unsafe RuntimeGenericParamInfoHandle (RuntimeStructs.GenericParamInfo* value)
		{
			this.value = value;
		}

		internal unsafe RuntimeGenericParamInfoHandle (IntPtr ptr)
		{
			this.value = (RuntimeStructs.GenericParamInfo*) ptr;
		}


		internal Type[] Constraints { get { return GetConstraints (); } }

		internal GenericParameterAttributes Attributes {
			get {
				unsafe {
					return (GenericParameterAttributes) value->flags;
				}
			}
		}

		Type[] GetConstraints () {
			int n = GetConstraintsCount ();
			var a = new Type[n];
			for (int i = 0; i < n; i++) {
				unsafe {
					RuntimeClassHandle c = new RuntimeClassHandle (value->constraints[i]);
					a[i] = Type.GetTypeFromHandle (c.GetTypeHandle ());
				}
			}
			return a;
		}

		int GetConstraintsCount () {
			int i = 0;
			unsafe {
				RuntimeStructs.MonoClass** p = value->constraints;
				while (p != null && *p != null)  {
					p++; i++;
				}
			}
			return i;
		}
	}
}
