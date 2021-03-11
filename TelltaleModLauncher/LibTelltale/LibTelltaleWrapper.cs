using System;
using System.Runtime.InteropServices;

/** 
 * C# Implementation of LibTelltale. May look slightly like Java because I don't code much C# D:
 * 
 * Written by LucasSaragosa
 * 
 * Members are left all protected but the classes are left final because in the future I may make then inheritable.
 * 
 * Remember that the library HAS to be called 'LibTelltale.dll' in the Dll directory as the exe or the one defined by SetDllDirectory.
 * 
 * MUST BE A 64 BIT APPLICATION TO USE LIBTELLTALE!!
 * 
*/

namespace LibTelltale {

	public class MemoryHelper {
		/// <summary>
		/// Represents bytes which have been read from a stream, which are eligible to be freed (save RAM :D). Use FreeReadBytes
		/// </summary>
		public struct Bytes
		{
			/// <summary>
			/// The backend memory ptr
			/// </summary>
			public IntPtr mem;
			/// <summary>
			/// The bytes which you read
			/// </summary>
			public byte[] bytes;
		}

		/// <summary>
		/// Frees the bytes read by Read.
		/// </summary>
		public static void FreeReadBytes(Bytes? b){
			if (b.HasValue) {
				Native.hMemory_Free (b.GetValueOrDefault ().mem);
			}
		}

	}

	public class Config {

		/// <summary>
		/// The Minimum version required of the LibTelltale DLL for this library to work.
		/// </summary>
		public static readonly Version MIN_VERSION = Version.Parse("2.5.2");

		/// <summary>
		/// If the given game ID uses the old telltale tool, for games before and including Game of Thrones.
		/// </summary>
		public static readonly uint GAMEFLAG_OLD_TELLTALE_TOOL = 1;

		/// <summary>
		/// If the given game ID uses .ttarch2 archives
		/// </summary>
		public static readonly uint GAMEFLAG_TTARCH2 = 2;

		/// <summary>
		/// If the given game ID uses .ttarch archives.
		/// </summary>
		public static readonly uint GAMEFLAG_TTARCH = 4;

		/// <summary>
		/// Meta Stream Version 5. All games using .ttarch2 up to Minecraft: Story Mode (by date)
		/// </summary>
		public static readonly uint META_V5 =  0x4D535635;
		/// <summary>
		/// Meta Stream Version 6. The most recent and probably not going to change. All games in .ttarch2 from Minecraft: Story Mode
		/// </summary>
		public static readonly uint META_V6 =  0x4D535636;
		/// <summary>
		/// Meta Stream Version 2, Meta Binary. Texas hold'em, CSI: 3 Dimensions of Murder, Bone
		/// </summary>
		public static readonly uint META_BIN = 0x4D42494E;
		/// <summary>
		/// Meta Stream version 3. All games using .ttarch after meta binary games
		/// </summary>
		public static readonly uint META_TRE = 0x4D545245;

		public static readonly uint OPEN_OK = 0;
		/// <summary>
		/// The metastreamed file has a bad format and could not be opened. Contact me on github to report it, along with the game its from and its file name.
		/// </summary>
		public static readonly uint OPEN_BAD_FORMAT = 1;
		/// <summary>
		/// Can happen when a CRC (hash) is not recognised. This is likely the most common error. In a meta stream, the type name will default to 'unknown_t'
		/// </summary>
		public static readonly uint OPEN_CRC_UNIMPL = 2;
		/// <summary>
		/// You passed bad or null arguments to the Open function
		/// </summary>
		public static readonly uint OPEN_BAD_ARGS = 3;
		/// <summary>
		/// When you attempt to load a .vers (MetaStreamedFile_Vers) which is already loaded or one with the same structure is.
		/// </summary>
		public static readonly uint OPEN_VERS_ALREADY_LOADED = 4;

		static Config() {
			if (MIN_VERSION > Version.Parse (GetVersion ()))
				throw new LibTelltaleException (String.Format("Cannot use LibTelltale v{0}, the minimum version required is v{1}", GetVersion(), MIN_VERSION));
		}

		/// <summary>
		/// Clears the mapped libraries.
		/// </summary>
		public static void ClearMappedLibraries(){
			Config0.LibTelltale_ClearMappedLibs ();
		}

		/// <summary>
		/// Maps a library. The library id can be found in libtelltale.h on github, and the dll_name is the dll file name that libtelltale should search using.
		/// </summary>
		/// <param name="lib_id">Lib identifier.</param>
		/// <param name="dll_name">Dll name.</param>
		public static void MapLibrary(string lib_id, string dll_name){
			if (lib_id == null || dll_name == null)
				return;
			Config0.LibTelltale_MapLib (Marshal.StringToHGlobalAnsi (lib_id), Marshal.StringToHGlobalAnsi (dll_name));
		}

		/// <summary>
		/// Gets the game flags for a given game id, returning 0 if the game id is invalid. 
		/// </summary>
		public static uint GetGameFlags(string gameid){
			return Config0.LibTelltale_GetGameFlags (gameid);
		}

		/// <summary>
		/// Gets the game archive version from the given game flags which can be returned by get game flags.
		/// </summary>
		public static uint GetGameArchiveVersion(uint gameFlags){
			return gameFlags >> 7;
		}

		/// <summary>
		/// Gets a game encryption key by its ID.
		/// </summary>
		/// <returns>The game encryption key.</returns>
		/// <param name="game_id">Game identifier.</param>
		public static string GetGameEncryptionKey(string game_id){
			return Marshal.PtrToStringAnsi (Config0.LibTelltale_GetKey (Marshal.StringToHGlobalAnsi (game_id)));
		}

		/// <summary>
		/// Gets the version of LibTelltale.
		/// </summary>
		/// <returns>The version.</returns>
		public static string GetVersion(){
			return Marshal.PtrToStringAnsi (Config0.LibTelltale_Version ());
		}

		/// <summary>
		/// Encrypts using the blowfish algorithm the given data with the given game ID encryption key. 
		/// The modified boolean is for the new modified algorithm telltale wrote for all .ttarch2 archives, and .ttarch archives with version 7+. (Open a .ttarch see the first byte).
		/// </summary>
		public static MemoryHelper.Bytes BlowfishEncrypt(byte[] data, string gameID, bool modified){
			MemoryHelper.Bytes ret = new MemoryHelper.Bytes ();
			ret.mem = Native.hMemory_Alloc ((uint)data.Length);
			Marshal.Copy (data, 0, ret.mem, data.Length);
			IntPtr tmp = Marshal.StringToHGlobalAnsi(gameID);
			Config0.LibTelltale_BlowfishEncrypt (ret.mem, (uint)data.Length, (byte)(modified ? 1 : 0), Config0.LibTelltale_GetKey(tmp));
			Marshal.FreeHGlobal (tmp);
			ret.bytes = new byte[data.Length];
			Marshal.Copy (ret.mem, ret.bytes, 0, data.Length);
			return ret;
		}

		/// <summary>
		/// Convert a plain text resource description lua script to an encrypted one able to be read by any telltale game.
		/// The modified boolean is for the new modified algorithm telltale wrote for all .ttarch2 archives, and .ttarch archives with version 7+. (Open a .ttarch see the first byte).
		/// The islenc boolean specifies if the file is a .lenc, which uses a slightly different format.
		/// </summary>
		public static MemoryHelper.Bytes EncryptResourceDescription(byte[] data, string gameID, bool modified, bool islenc){
            unsafe
            {
				MemoryHelper.Bytes ret = new MemoryHelper.Bytes ();
				ret.mem = Native.hMemory_Alloc ((uint)data.Length);
				Marshal.Copy (data, 0, ret.mem, data.Length);
				IntPtr tmp = Marshal.StringToHGlobalAnsi (gameID);
				uint i = 0;
				uint* outz = &i;
				IntPtr ret1 = Config0.LibTelltale_EncryptResourceDescription (ret.mem, (uint)data.Length, outz, (byte)(modified ? 1 : 0), Config0.LibTelltale_GetKey (tmp), (byte)(islenc ? 1 : 0));
				Marshal.FreeHGlobal (tmp);
				Native.hMemory_Free (ret.mem);
				ret.mem = ret1;
				ret.bytes = new byte[*outz];
				Marshal.Copy (ret.mem, ret.bytes, 0, (int)*outz);
				return ret;
			}
		}
			
		/// <summary>
		/// Convert an encrypted lua script to an unencrypted plain text one.
		/// The modified boolean is for the new modified algorithm telltale wrote for all .ttarch2 archives, and .ttarch archives with version 7+. (Open a .ttarch see the first byte).
		/// </summary>
		public static MemoryHelper.Bytes DecryptResourceDescription(byte[] data, string gameID, bool modified){
			unsafe {
				MemoryHelper.Bytes ret = new MemoryHelper.Bytes ();
				ret.mem = Native.hMemory_Alloc ((uint)data.Length);
				Marshal.Copy (data, 0, ret.mem, data.Length);
				IntPtr tmp = Marshal.StringToHGlobalAnsi (gameID);
				uint i = 0;
				uint* outz = &i;
				IntPtr ret1 = Config0.LibTelltale_DecryptResourceDescription (ret.mem, (uint)data.Length, outz, (byte)(modified ? 1 : 0), Config0.LibTelltale_GetKey (tmp));
				Marshal.FreeHGlobal (tmp);
				Native.hMemory_Free (ret.mem);
				ret.mem = ret1;
				ret.bytes = new byte[*outz];
				Marshal.Copy (ret.mem, ret.bytes, 0, (int)*outz);
				return ret;
			}
		}

		public static MemoryHelper.Bytes EncryptScript(byte[] data, string gameID, bool modified, bool islenc){
			MemoryHelper.Bytes ret = new MemoryHelper.Bytes ();
			ret.mem = Native.hMemory_Alloc ((uint)data.Length);
			Marshal.Copy (data, 0, ret.mem, data.Length);
			IntPtr tmp = Marshal.StringToHGlobalAnsi (gameID);
			IntPtr ret1 = Config0.LibTelltale_EncryptScript (ret.mem, (uint)data.Length, (byte)(modified ? 1 : 0), Config0.LibTelltale_GetKey (tmp), (byte)(islenc ? 1 : 0));
			Marshal.FreeHGlobal (tmp);
			Native.hMemory_Free (ret.mem);
			ret.mem = ret1;
			ret.bytes = new byte[data.Length];
			Marshal.Copy (ret.mem, ret.bytes, 0, data.Length);
			return ret;
		}

		public static MemoryHelper.Bytes DecryptScript(byte[] data, string gameID, bool modified){
			MemoryHelper.Bytes ret = new MemoryHelper.Bytes ();
			ret.mem = Native.hMemory_Alloc ((uint)data.Length);
			Marshal.Copy (data, 0, ret.mem, data.Length);
			IntPtr tmp = Marshal.StringToHGlobalAnsi (gameID);
			IntPtr ret1 = Config0.LibTelltale_DecryptScript (ret.mem, (uint)data.Length,  (byte)(modified ? 1 : 0), Config0.LibTelltale_GetKey (tmp));
			Marshal.FreeHGlobal (tmp);
			Native.hMemory_Free (ret.mem);
			ret.mem = ret1;
			ret.bytes = new byte[data.Length];
			Marshal.Copy (ret.mem, ret.bytes, 0, data.Length);
			return ret;
		}

		/// <summary>
		/// Decrypts using the blowfish algorithm the given data with the given game ID encryption key. 
		/// The modified boolean is for the new modified algorithm telltale wrote for all .ttarch2 archives, and .ttarch archives with version 7+. (Open a .ttarch see the first byte).
		/// </summary>
		public static MemoryHelper.Bytes BlowfishDecrypt(byte[] data, string gameID, bool modified){
			MemoryHelper.Bytes ret = new MemoryHelper.Bytes ();
			ret.mem = Native.hMemory_Alloc ((uint)data.Length);
			Marshal.Copy (data, 0, ret.mem, data.Length);
			IntPtr tmp = Marshal.StringToHGlobalAnsi(gameID);
			Config0.LibTelltale_BlowfishDecrypt (ret.mem, (uint)data.Length, (byte)(modified ? 1 : 0), Config0.LibTelltale_GetKey(tmp));
			Marshal.FreeHGlobal (tmp);
			ret.bytes = new byte[data.Length];
			Marshal.Copy (ret.mem, ret.bytes, 0, data.Length);
			return ret;
		}

	}

	class Config0 {

		[DllImport("LibTelltale.dll")]
		public static extern uint LibTelltale_GetGameFlags([MarshalAs(UnmanagedType.LPStr)] string gameid);

		[DllImport("LibTelltale.dll")]
		public static extern void LibTelltale_BlowfishEncrypt (IntPtr data, uint size, byte modified, IntPtr k);

		[DllImport("LibTelltale.dll")]
		public static extern void LibTelltale_BlowfishDecrypt (IntPtr data, uint size, byte modified, IntPtr k);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr LibTelltale_DecryptScript(IntPtr data, uint size, byte modified, IntPtr k);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr LibTelltale_EncryptScript(IntPtr data, uint size, byte modified, IntPtr k, byte islenc);

		[DllImport("LibTelltale.dll")]
		public static extern unsafe IntPtr LibTelltale_DecryptResourceDescription(IntPtr data, uint size, uint* outz, byte modified, IntPtr k);

		[DllImport("LibTelltale.dll")]
		public static extern unsafe IntPtr LibTelltale_EncryptResourceDescription(IntPtr data, uint size, uint* outz, byte modified, IntPtr k, byte islenc);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr LibTelltale_Version();

		[DllImport("LibTelltale.dll")]
		public static extern void LibTelltale_MapLib(IntPtr id, IntPtr name);

		[DllImport("LibTelltale.dll")]
		public static extern void LibTelltale_ClearMappedLibs ();

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr LibTelltale_GetKey(IntPtr id);

	}

	/// <summary>
	/// Any exception which is because you may have forgotten to set a stream etc.
	/// </summary>
	public class LibTelltaleException : Exception {
		public LibTelltaleException(string message) : base(message) {}
	}

	/// <summary>
	/// This namespace is all to do with the loading/writing of TTArchive bundles, .ttarch and .ttarch2
	/// </summary>
	namespace TTArchives {

		public struct TTArchiveOrTTArchive2 {
			public byte isTTArchive2;
			public IntPtr archive;
			public IntPtr archive2;

			public static TTArchiveOrTTArchive2 CreateFromArchive(TTArchive archive){
				TTArchiveOrTTArchive2 ret = new TTArchiveOrTTArchive2 ();
				ret.archive = archive.reference;
				ret.archive2 = IntPtr.Zero;
				ret.isTTArchive2 = 1;
				return ret;
			}

			public static TTArchiveOrTTArchive2 CreateFromArchive(TTArchive2 archive){
				TTArchiveOrTTArchive2 ret = new TTArchiveOrTTArchive2 ();
				ret.archive2 = archive.reference;
				ret.archive = IntPtr.Zero;
				ret.isTTArchive2 = 2;
				return ret;
			}

		}

		/// <summary>
		/// Constants which get returned from flush and open, use this to set custom options too.
		/// </summary>
		public static class Constants {
			public static readonly uint TTARCH_OPEN_OK = 0;
			public static readonly uint TTARCH_OPEN_BAD_STREAM = 1;
			public static readonly uint TTARCH_OPEN_BAD_HEADER = 2;
			public static readonly uint TTARCH_OPEN_BAD_VERSION = 4;
			public static readonly uint TTARCH_OPEN_BAD_DATA = 5;
			public static readonly uint TTARCH_OPEN_BAD_KEY = 6;
			public static readonly uint TTARCH_OPEN_LIB_ERR = 7;
			public static readonly uint TTARCH_OPEN_BAD_ARCHIVE = 8;
			public static readonly uint TTARCH_FLUSH_OK = 0;
			public static readonly uint TTARCH_FLUSH_BAD_STREAM = 1;
			public static readonly uint TTARCH_FLUSH_BAD_ARCHIVE = 2;
			public static readonly uint TTARCH_FLUSH_DATA_ERR = 3;
			public static readonly uint TTARCH_FLUSH_LIB_ERR = 4;
			public static readonly uint TTARCH_FLUSH_BAD_OPTIONS = 5;
			public static readonly uint TTARCH_FLUSH_COMPRESS_DEFAULT = 1;
			public static readonly uint TTARCH_FLUSH_COMPRESS_OODLE = 2;
			public static readonly uint TTARCH_FLUSH_ENCRYPT = 4;
			public static readonly uint TTARCH_FLUSH_SKIP_CRCS =	8;
			public static readonly uint TTARCH_FLUSH_RAW = 16;
			public static readonly uint TTARCH_FLUSH_NO_TMPFILE = 32;
			public static readonly uint TTARCH_FLUSH_V0 = 0;
			public static readonly uint TTARCH_FLUSH_V1 = 128;
			public static readonly uint TTARCH_FLUSH_V2 = 256;
			public static readonly uint TTARCH_FLUSH_V3 = 384;
			public static readonly uint TTARCH_FLUSH_V4 = 512;
			public static readonly uint TTARCH_FLUSH_V7 = 896;
			public static readonly uint TTARCH_FLUSH_V8 = 1024;
			public static readonly uint TTARCH_FLUSH_V9 = 1152;
		}

		/// <summary>
		/// Represents an entry in a .ttarch archive. Do not touch the referencee pointer or any value in any of the fields! The only field useful is the backend.name.
		/// </summary>
		public struct TTArchiveEntry {
			public _TTArchiveEntry backend;
			public IntPtr reference;
		}

		/// <summary>
		/// The full struct of an entry in memory, important that you do not edit fields in here!
		/// </summary>
		public struct _TTArchiveEntry {
			public IntPtr override_stream; // To override it, use set stream!
			public ulong offset;
			public uint size;
			[MarshalAs(UnmanagedType.LPStr)]
			public string name;
			public ulong name_crc;
			public byte flags;
		}

		/// <summary>
		/// Handles a .ttarch archive
		/// </summary>
		public sealed class TTArchive : IDisposable {

			/// <summary>
			/// Sets the name of the entry, do not use entry.name = ...
			/// </summary>
			public static void SetEntryName(TTArchiveEntry entry, string name){
				Native.hTTArchive_EntrySetName (entry.reference, name);
			}

			/// <summary>
			/// Creates the TTArchive entry, using the given name and input stream of bytes.
			/// </summary>
			/// <returns>The TTArchive entry.</returns>
			public static TTArchiveEntry CreateTTArchiveEntry(string name, ByteStream stream){
				TTArchiveEntry ret = new TTArchiveEntry();
				ret.reference = Native.TTArchive_EntryCreate (name, stream == null ? IntPtr.Zero : stream.reference);
				ret.backend = (_TTArchiveEntry)Marshal.PtrToStructure (ret.reference, typeof(_TTArchiveEntry));
				return ret;
			}

			[StructLayout(LayoutKind.Sequential)]
			protected struct ttarch {
				public IntPtr game_key;
				public IntPtr entries;
				public IntPtr stream;
				public IntPtr flushstream;
				public IntPtr reserved;
				public uint options;
			};

			protected ttarch handle;

			/// <summary>
			/// Reference pointer, do not touch.
			/// </summary>
			public readonly IntPtr reference;

			protected readonly string gameID;

			public string GetGameID(){
				return this.gameID;
			}

			protected ByteStream instream;

			protected ByteOutStream outstream;

			/// <summary>
			/// Shortcut to get the name string of the entry.
			/// </summary>
			public string GetEntryName(TTArchiveEntry e){
				return e.backend.name;
			}

			/// <summary>
			/// Gets the archive options.
			/// </summary>
			/// <returns>The archive options.</returns>
			public uint GetArchiveOptions(){
				UpdateAndSync (true);
				return this.handle.options;
			}

			/// <summary>
			/// Sets an archive option, use the options from constants.
			/// </summary>
			/// <param name="op">Op.</param>
			public void SetArchiveOption(uint op){
				this.handle.options |= op;
				UpdateAndSync (false);
			}

			/// <summary>
			/// Convenience method to unset an option from constants.
			/// </summary>
			/// <param name="op">Op.</param>
			public void UnsetArchiveOption(uint op){
				this.handle.options &= ~op;
				UpdateAndSync (false);
			}

			/// <summary>
			/// Gets the archive version.
			/// </summary>
			/// <returns>The archive version.</returns>
			public uint GetArchiveVersion(){
				return (GetArchiveOptions () >> 7) & 15;
			}

			/// <summary>
			/// Sets the archive version. Must only use versions from constants.
			/// </summary>
			/// <param name="version">Version.</param>
			public void SetArchiveVersion(uint version){
				this.handle.options &= 127;
				this.handle.options |= (version & 15) << 7;
				UpdateAndSync (false);
			}

			/// <summary>
			/// Initializes a new  <see cref="LibTelltale.TTArchives.TTArchive"/>. The game ID should be the game ID from the github page, and will throw an exception in the case that the game for the ID
			/// could not be found.
			/// </summary>
			/// <param name="gameid">The Game ID</param>
			public TTArchive(string gameid){
				reference = Native.hTTArchive_Create ();
				if (this.reference.Equals (IntPtr.Zero))
					throw new LibTelltaleException ("Could not create backend archive");	
				this.UpdateAndSync (true);
				IntPtr key = Marshal.StringToHGlobalAnsi (gameid);
				this.handle.game_key = Config0.LibTelltale_GetKey (key);
				if (this.handle.game_key.Equals (IntPtr.Zero))
					throw new LibTelltaleException (String.Format("Could not find a key for the game ID {0}", gameid));
				Marshal.FreeHGlobal (key);
				this.gameID = gameid;
				this.SetArchiveVersion (Config.GetGameArchiveVersion (Config.GetGameFlags (gameid)));
				this.UpdateAndSync (false);
			}

			/// <summary>
			/// Opens a readable byte input stream for the given entry.
			/// </summary>
			public ByteStream StreamOpen(TTArchiveEntry entry){
				return new ByteStream (Native.TTArchive_StreamOpen (reference, entry.reference));
			}

			/// <summary>
			/// Removes the entry from the archive.
			/// </summary>
			/// <param name="e">The Entry to remove. Must not be null</param>
			/// <param name="delete">If set to <c>true</c> then the entry will be freed to save memory. Use this only if you are removing and deleting the entry because you don't need it</param>
			public void RemoveEntry(TTArchiveEntry e, bool delete){
				Native.hTTArchive_EntryRemove (this.reference, e.reference, delete);
			}

			/// <summary>
			/// Finds an entry by its full file name, for example FindEntry("Boot.lua"). Returns a nullable value, if not found.
			/// </summary>
			public TTArchiveEntry? FindEntry(string name){
				TTArchiveEntry r = new TTArchiveEntry ();	
				IntPtr entryp = Native.TTArchive_EntryFind(reference, name);
				if (entryp.Equals (IntPtr.Zero))
					return null;
				r.backend = (_TTArchiveEntry)Marshal.PtrToStructure (entryp, typeof(_TTArchiveEntry));
				r.reference = entryp;
				return r;
			}

			/// <summary>
			/// Sets the stream for entry, which will be the overriden stream to use when writing (flushing) the archive.
			/// </summary>
			public void SetStreamForEntry(TTArchiveEntry entry, ByteStream stream){
				Native.TTArchive_StreamSet (entry.reference, stream.reference);
				entry.backend = (_TTArchiveEntry)Marshal.PtrToStructure (entry.reference, typeof(_TTArchiveEntry));
			}

			/// <summary>
			/// Adds the entry to the backend vector list ready for when you write back the archive.
			/// </summary>
			/// <param name="entry">Entry.</param>
			public void AddEntry(TTArchiveEntry entry){
				Native.hTTArchive_EntryAdd (this.reference, entry.reference);
				UpdateAndSync (true);
			}

			/// <summary>
			/// Gets an entry at the given file index. This is used when you want to loop through all files, or if you already know the index in the entry list.
			/// </summary>
			/// <returns>The entry.</returns>
			public TTArchiveEntry? GetEntry(uint index){
				TTArchiveEntry r = new TTArchiveEntry ();
				IntPtr entryp = Native.hTTArchive_GetEntryAt (reference, index);
				if (entryp.Equals (IntPtr.Zero))
					return null;
				r.backend = (_TTArchiveEntry)Marshal.PtrToStructure (entryp, typeof(_TTArchiveEntry));
				r.reference = entryp;
				return r;
			}

			/// <summary>
			/// Clears the entries.
			/// </summary>
			public void ClearEntries(){
				Native.hTTArchive_ClearEntries (this.reference);
				UpdateAndSync (true);
			}

			/// <summary>
			/// Writes all entries with the given options to the outstream in this archive instance. Returns TTARCH_FLUSH_OK is all goes well (0), else see the constants value.
			/// </summary>
			public int Flush(){
				if (outstream == null)
					throw new LibTelltaleException ("No stream set");
				int ret = Native.TTArchive_Flush (this.reference,IntPtr.Zero);
				if (ret == Constants.TTARCH_FLUSH_OK) {
					UpdateAndSync (true);
				}
				return ret;
			}

			/// <summary>
			/// Releases all resources used by this <see cref="LibTelltale.TTArchives.TTArchive"/> object.
			/// This also frees all memory within the backend TTArchive and all its entries so make sure you call this after you need every entry otherwise you will get unmanaged memory errors.
			/// </summary>
			public void Dispose(){
				Native.TTArchive_Free (this.reference);
				Native.hTTArchive_Delete (this.reference);
				this.instream = null;
				this.outstream = null;
			}

			/// <summary>
			/// Opens and adds all entries from the given archive InStream. Do not use when you have previously loaded an archive, in that case use a new instance of this class.
			/// </summary>
			public int Open(){
				if (instream == null)
					throw new LibTelltaleException ("No stream set");
				int ret = Native.TTArchive_Open (this.reference);
				if (ret == Constants.TTARCH_OPEN_OK) {
					UpdateAndSync (true);
				}
				return ret;
			}

			/// <summary>
			/// The amount of entries in this archive.
			/// </summary>
			/// <returns>The entry count.</returns>
			public uint GetEntryCount(){
				return Native.hTTArchive_GetEntryCount (reference);
			}
				
			/// <summary>
			/// Gets or sets the input stream to read the archive from in Open
			/// </summary>
			/// <value>The in stream.</value>
			public ByteStream InStream {
				get{ return instream; }
				set { instream = value;  if(instream != null)this.handle.stream = instream.reference;  if(instream != null)UpdateAndSync (false); }
			}

			/// <summary>
			/// Gets or sets the output stream to write the archive to in Flush.
			/// </summary>
			/// <value>The out stream.</value>
			public ByteOutStream OutStream {
				get{ return outstream; }
				set { outstream = value; if(outstream != null)this.handle.flushstream = outstream.reference;  if(outstream != null)UpdateAndSync (false);}
			}

			protected void UpdateAndSync(bool retrieve){
				if (retrieve) {
					this.handle = (ttarch)Marshal.PtrToStructure (reference, typeof(ttarch));
				} else {
					Marshal.StructureToPtr (this.handle, reference, false);
				}
			}

		}

		/// <summary>
		/// Represents a file entry in a TTArchive2 (.ttarch2) archive.
		/// </summary>
		public struct TTArchive2Entry {
			public _TTArchive2Entry backend;
			public IntPtr reference;
		}

		/// <summary>
		/// Backend structure of an entry in memory. Do not edit fields, the only useful field in this struct is the name, which you can retrieve to your liking.
		/// </summary>
		public struct _TTArchive2Entry {
			public ulong offset;
			public uint size;
			public ulong name_crc;
			[MarshalAs(UnmanagedType.LPStr)]
			public string name;
			public IntPtr override_stream; // To override it, use set stream!
			public byte flags;
		}

		/// <summary>
		/// A .ttarch2 archive
		/// </summary>
		public sealed class TTArchive2 : IDisposable {

			/// <summary>
			/// Sets the name of the given entry. Do not use a direct set to the entry.backend.name!
			/// </summary>
			public static void SetEntryName(TTArchive2Entry entry, string name){
				Native.TTArchive2_EntrySetName (entry.reference, name);
			}
			 
			/// <summary>
			/// Creates a TTArchive2 entry, with the given name and input stream of bytes.
			/// </summary>
			/// <returns>The TT archive2 entry.</returns>
			public static TTArchive2Entry CreateTTArchive2Entry(string name, ByteStream stream){ 
				if (stream != null) {
					TTArchive2Entry ret = new TTArchive2Entry ();
					ret.reference = Native.TTArchive2_EntryCreate (name, stream.reference);
					ret.backend = (_TTArchive2Entry)Marshal.PtrToStructure (ret.reference, typeof(_TTArchive2Entry));
					return ret;
				} else {
					TTArchive2Entry ret = new TTArchive2Entry ();
					ret.reference = Native.TTArchive2_EntryCreate (name, IntPtr.Zero);
					ret.backend = (_TTArchive2Entry)Marshal.PtrToStructure (ret.reference, typeof(_TTArchive2Entry));
					return ret;
				}
			}

			[StructLayout(LayoutKind.Sequential)]
			protected struct ttarch2 {
				public uint options;
				public IntPtr game_key;
				public IntPtr entries;
				public IntPtr stream;
				public IntPtr flushstream;
				public byte flags;
			};

			protected ttarch2 handle;

			public readonly IntPtr reference;

			protected readonly string gameID;

			public string GetGameID(){
				return this.gameID;
			}

			protected ByteStream instream;

			protected ByteOutStream outstream;

			/// <summary>
			/// Gets the options for this archive.
			/// </summary>
			/// <returns>The archive options.</returns>
			public uint GetArchiveOptions(){
				UpdateAndSync (true);
				return this.handle.options;
			}

			/// <summary>
			/// Removes the entry from the archive.
			/// </summary>
			/// <param name="e">The Entry to remove. Must not be null</param>
			/// <param name="delete">If set to <c>true</c> then the entry will be freed to save memory. Use this only if you are removing and deleting the entry because you don't need it</param>
			public void RemoveEntry(TTArchive2Entry e, bool delete){
				Native.hTTArchive2_EntryRemove (this.reference, e.reference, delete);
			}

			/// <summary>
			/// Gets the archive version.
			/// </summary>
			/// <returns>The archive version.</returns>
			public uint GetArchiveVersion(){
				return (GetArchiveOptions () >> 7) & 15;
			}

			/// <summary>
			/// Sets the archive version.
			/// </summary>
			/// <param name="version">Version.</param>
			public void SetArchiveVersion(uint version){
				this.handle.options &= 127;
				this.handle.options |= (version & 15) << 7;
				UpdateAndSync (false);
			}

			/// <summary>
			/// Sets the archive an option from the constants class.
			/// </summary>
			/// <param name="op">Op.</param>
			public void SetArchiveOption(uint op){
				this.handle.options |= op;
				UpdateAndSync (false);
			}

			/// <summary>
			/// Unsets the archive option.
			/// </summary>
			/// <param name="op">Op.</param>
			public void UnsetArchiveOption(uint op){
				this.handle.options &= ~op;
				UpdateAndSync (false);
			}

			/// <summary>
			/// Initializes a new  <see cref="LibTelltale.TTArchives.TTArchive2"/>. The game ID should be the game ID from the github page, and will throw an exception in the case that the game for the ID
			/// could not be found.
			/// </summary>
			/// <param name="gameid">The Game ID</param>
			public TTArchive2(string gameid){
				reference = Native.hTTArchive2_Create ();
				if (this.reference.Equals (IntPtr.Zero))
					throw new LibTelltaleException ("Could not create backend archive");	
				this.UpdateAndSync (true);
				IntPtr key = Marshal.StringToHGlobalAnsi (gameid);
				this.handle.game_key = Config0.LibTelltale_GetKey (key);
				if (this.handle.game_key.Equals (IntPtr.Zero))
					throw new LibTelltaleException (String.Format("Could not find a key for the game ID {0}", gameid));
				Marshal.FreeHGlobal (key);
				this.gameID = gameid;
				this.SetArchiveVersion (Config.GetGameArchiveVersion (Config.GetGameFlags (gameid)));
				this.UpdateAndSync (false);
			}

			/// <summary>
			/// Opens a readable byte stream of the given entry.
			/// </summary>
			/// <returns>The open.</returns>
			/// <param name="entry">Entry.</param>
			public ByteStream StreamOpen(TTArchive2Entry entry){
				return new ByteStream (Native.TTArchive2_StreamOpen (this.reference, entry.reference));
			}

			/// <summary>
			/// Shortcut to get the name string of the entry.
			/// </summary>
			public string GetEntryName(TTArchive2Entry entry){
				return entry.backend.name;
			}

			/// <summary>
			/// Finds an entry by its name
			/// </summary>
			/// <returns>The entry.</returns>
			/// <param name="name">Name.</param>
			public TTArchive2Entry? FindEntry(string name){
				TTArchive2Entry r = new TTArchive2Entry ();	
				IntPtr entryp = Native.TTArchive2_EntryFind(reference, name);
				if (entryp.Equals (IntPtr.Zero))
					return null;
				r.backend = (_TTArchive2Entry)Marshal.PtrToStructure (entryp, typeof(_TTArchive2Entry));
				r.reference = entryp;
				return r;
			}

			/// <summary>
			/// Sets the input byte stream for the given entry.
			/// </summary>
			/// <param name="entry">Entry.</param>
			/// <param name="stream">Stream.</param>
			public void SetStreamForEntry(TTArchive2Entry entry, ByteStream stream){
				Native.TTArchive2_StreamSet (entry.reference, stream.reference);
				entry.backend = (_TTArchive2Entry)Marshal.PtrToStructure (entry.reference, typeof(_TTArchive2Entry));
			}

			/// <summary>
			/// Adds an entry to the archive
			/// </summary>
			/// <param name="entry">Entry.</param>
			public void AddEntry(TTArchive2Entry entry){
				Native.hTTArchive2_EntryAdd (this.reference, entry.reference);
				UpdateAndSync (true);
			}

			/// <summary>
			/// Gets an entry by its index in the archive entries backend list. Useful for iterating over all entries. It is nullable.
			/// </summary>
			/// <returns>The entry.</returns>
			/// <param name="index">Index.</param>
			public TTArchive2Entry? GetEntry(uint index){
				TTArchive2Entry r = new TTArchive2Entry ();
				IntPtr entryp = Native.hTTArchive2_GetEntryAt (reference, index);
				if (entryp.Equals (IntPtr.Zero))
					return null;
				r.backend = (_TTArchive2Entry)Marshal.PtrToStructure (entryp, typeof(_TTArchive2Entry));
				r.reference = entryp;
				return r;
			}

			/// <summary>
			/// Clears the entries in this archive.
			/// </summary>
			public void ClearEntries(){
				Native.hTTArchive2_ClearEntries (this.reference);
				UpdateAndSync (true);
			}

			/// <summary>
			/// Writes all entries with the options set in this archive to the OutStream as a valid .ttarch2.
			/// </summary>
			public int Flush(){
				if (outstream == null)
					throw new LibTelltaleException ("No stream set");
				int ret = Native.TTArchive2_Flush (this.reference,IntPtr.Zero);
				if (ret == Constants.TTARCH_FLUSH_OK) {
					UpdateAndSync (true);
				}
				return ret;
			}

			/// <summary>
			/// Releases all resources used by this <see cref="LibTelltale.TTArchives.TTArchive2"/> object.
			/// This also frees all memory within the backend TTArchive and all its entries so make sure you call this after you need every entry otherwise you will get unmanaged memory errors.
			/// </summary>
			public void Dispose(){
				Native.TTArchive2_Free (this.reference);
				Native.hTTArchive2_Delete (this.reference);
				this.instream = null;
				this.outstream = null;
			}

			/// <summary>
			/// Opens and adds all entries from the given .ttarch archive input stream. This also sets the options, and allows once the outstream is set for you to write back the archive to a stream.
			/// </summary>
			public int Open(){
				if (instream == null)
					throw new LibTelltaleException ("No stream set");
				int ret = Native.TTArchive2_Open (this.reference);
				if (ret == Constants.TTARCH_OPEN_OK) {
					UpdateAndSync (true);
				}
				return ret;
			}

			/// <summary>
			/// Gets the amount of entries in this archive.
			/// </summary>
			/// <returns>The entry count.</returns>
			public uint GetEntryCount(){
				return Native.hTTArchive2_GetEntryCount (reference);
			}

			/// <summary>
			/// Gets or sets the input stream to read the archive from in Open.
			/// </summary>
			/// <value>The in stream.</value>
			public ByteStream InStream {
				get{ return instream; }
				set { instream = value;  if(instream != null)this.handle.stream = instream.reference;  if(instream != null)UpdateAndSync (false); }
			}

			/// <summary>
			/// Gets or sets the output stream to write the archive to in Flush.
			/// </summary>
			/// <value>The out stream.</value>
			public ByteOutStream OutStream {
				get{ return outstream; }
				set { outstream = value; if(outstream != null)this.handle.flushstream = outstream.reference;  if(outstream != null)UpdateAndSync (false);}
			}

			protected void UpdateAndSync(bool retrieve){
				if (retrieve) {
					this.handle = (ttarch2)Marshal.PtrToStructure (reference, typeof(ttarch2));
				} else {
					Marshal.StructureToPtr (this.handle, reference, false);
				}
			}

		}

	}

	/// <summary>
	/// An input stream of bytes which can be read from and positioned.
	/// </summary>
	public sealed class ByteStream : IDisposable {

		public void Dispose(){
			Native.hByteStream_Delete (this.reference);
		}

		/// <summary>
		/// The reference, do not touch!
		/// </summary>
		public readonly IntPtr reference = IntPtr.Zero;

		/// <summary>
		/// Used internally to create from the given stream pointer.
		/// </summary>
		/// <param name="r">The red component.</param>
		public ByteStream(IntPtr r){
			this.reference = r;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LibTelltale.ByteStream"/> class. The filepath is the existing file location of the file to read from.
		/// </summary>
		/// <param name="filepath">Filepath.</param>
		public ByteStream(string filepath){
			this.reference = Native.hFileStream_Create (filepath);
			if (this.reference.Equals (IntPtr.Zero) || !IsValid())
				throw new LibTelltaleException ("Could not create backend filestream");	
		}

		/// <summary>
		/// Determines whether this instance is valid, and if it is not then the archive will return a stream error. This is if the backend byte buffer is non existent or the file does not exist.
		/// </summary>
		/// <returns><c>true</c> if this instance is valid; otherwise, <c>false</c>.</returns>
		public bool IsValid(){
			return Native.hByteStream_Valid (reference) != 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LibTelltale.ByteStream"/> class, with the given size of zeros. Useful if you need an empty stream.
		/// </summary>
		/// <param name="initalSize">Inital size.</param>
		public ByteStream(uint initalSize){
			this.reference = Native.hByteStream_Create (initalSize);
			if (this.reference.Equals (IntPtr.Zero))
				throw new LibTelltaleException ("Could not create backend stream");	
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LibTelltale.ByteStream"/> class, which reads from the given input byte buffer. Do not set the byte array to null until you are done with this stream.
		/// </summary>
		/// <param name="buffer">Buffer.</param>
		public ByteStream(byte[] buffer){
			IntPtr ptr = Marshal.AllocHGlobal (buffer.Length);
			Marshal.Copy (buffer, 0, ptr, buffer.Length);
			this.reference = Native.hByteStream_CreateFromBuffer (ptr, (uint)buffer.Length);
			if (this.reference.Equals (IntPtr.Zero))
				throw new LibTelltaleException ("Could not create backend stream");	
		}

		/// <summary>
		/// Reads a single byte.
		/// </summary>
		/// <returns>The single byte.</returns>
		public byte ReadSingleByte(){
			return Native.hByteStream_ReadByte (reference);
		}

		/// <summary>
		/// Gets the backend buffer this stream is reading from, or null if there is none (for example if its reading from a file).
		/// </summary>
		/// <returns>The buffer.</returns>
		public byte[] GetBuffer(){
			IntPtr ptr = Native.hByteStream_GetBuffer (reference);
			if (ptr.Equals (IntPtr.Zero))
				return null;
			byte[] ret = new byte[GetSize ()];
			Marshal.Copy (ptr,ret, 0, (int)GetSize ());
			//Dont want to free the buffer!!
			return ret;
		}
			
		/// <summary>
		/// Read the specified amount of bytes, and increases the position by length. Returns a byte struct which allows this memory to later be freed once your done with it.
		/// </summary>
		/// <param name="length">Length.</param>
		public MemoryHelper.Bytes? Read(int length){
			IntPtr read = Native.hByteStream_ReadBytes (reference, (uint)length);
			if (read.Equals (IntPtr.Zero))
				return null;
			byte[] ret1 = new byte[length];
			Marshal.Copy (read, ret1, 0, length);
			MemoryHelper.Bytes ret = new MemoryHelper.Bytes ();
			ret.bytes = ret1;
			ret.mem = read;
			return ret;
		}

		/// <summary>
		/// Reads an unsigned integer of the specified bit width. Valid widths are <= 64 and a multiple of eight and >= 8.
		/// </summary>
		public ulong ReadInt(byte bit_width){
			return Native.hByteStream_ReadInt(reference, bit_width);
		}

		/// <summary>
		/// Reads a null terminated string.
		/// </summary>
		/// <returns>The null terminated string.</returns>
		public string ReadNullTerminatedString(){
			return Marshal.PtrToStringAnsi (Native.hByteStream_ReadString0 (reference));
		} 

		/// <summary>
		/// Reads an ASCII string of the given byte length
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="len">Length.</param>
		public string ReadString(uint len){
			return Marshal.PtrToStringAnsi(Native.hByteStream_ReadString(reference, len));
		}

		/// <summary>
		/// Determines whether this instance is little endian.
		/// </summary>
		/// <returns><c>true</c> if this instance is little endian; otherwise, <c>false</c>.</returns>
		public bool IsLittleEndian(){
			return Native.hByteStream_IsLittleEndian (reference) != 0;
		}

		/// <summary>
		/// Positions to the specified offset.
		/// </summary>
		/// <param name="offset">Offset.</param>
		public void Position(ulong offset){
			Native.hByteStream_Position (reference, offset);
		}

		/// <summary>
		/// Sets the endianness of the stream to read ints by.
		/// </summary>
		/// <param name="little">If set to <c>true</c> little.</param>
		public void SetEndian(bool little){
			Native.hByteStream_SetEndian (reference, little);
		}

		/// <summary>
		/// Gets the position.
		/// </summary>
		/// <returns>The position.</returns>
		public ulong GetPosition(){
			return Native.hByteStream_GetPosition (reference);
		}

		/// <summary>
		/// Gets the size of this stream.
		/// </summary>
		/// <returns>The size.</returns>
		public int GetSize(){
			return (int)Native.hByteStream_GetSize (reference);
		}

	}

	/// <summary>
	/// An output stream of bytes
	/// </summary>
	public sealed class ByteOutStream : IDisposable {

		//Do not touch this!
		public readonly IntPtr reference = IntPtr.Zero;

		/// <summary>
		/// Initializes a new instance of the <see cref="LibTelltale.ByteOutStream"/> class, which writes to the passed in file which doesnt have to exist. However its directory must exist of undefined behaviour!
		/// </summary>
		/// <param name="filepath">Filepath.</param>
		public ByteOutStream(string filepath){
			this.reference = Native.hFileOutStream_Create (filepath);
			if (this.reference.Equals (IntPtr.Zero) || !IsValid())
				throw new LibTelltaleException ("Could not create backend filestream");	
		}

		/// <summary>
		/// Internal use
		/// </summary>
		/// <param name="ptr">Ptr.</param>
		public ByteOutStream(IntPtr ptr){
			this.reference = ptr;
		}

		/// <summary>
		/// Determines whether this stream is valid.
		/// </summary>
		/// <returns><c>true</c> if this instance is valid; otherwise, <c>false</c>.</returns>
		public bool IsValid(){
			return Native.hByteOutStream_Valid (reference) != 0;
		}

		/// <summary>
		/// Writes an unsigned int to this stream. You will have to cast to a ulong on calling if not a uint64!
		/// </summary>
		/// <param name="bit_width">Bit width.</param>
		/// <param name="num">Number.</param>
		public void WriteInt(byte bit_width, ulong num){
			Native.hByteOutStream_WriteInt (reference, bit_width, num);
		}

		/// <summary>
		/// Write the specified buffer to this stream.
		/// </summary>
		/// <param name="buf">Buffer.</param>
		public void Write(byte[] buf){
			IntPtr ptr = Native.hMemory_Alloc ((uint)buf.Length);
			Marshal.Copy (buf, 0, ptr, buf.Length);
			Native.hByteOutStream_WriteBytes (reference, ptr, (uint)buf.Length);
			Native.hMemory_Free (ptr);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LibTelltale.ByteOutStream"/> class, with the initial buffer size to save time reallocating the memory.
		/// </summary>
		/// <param name="initalSize">Inital size.</param>
		public ByteOutStream(uint initalSize){
			this.reference = Native.hByteOutStream_Create (initalSize);
			if (this.reference.Equals (IntPtr.Zero))
				throw new LibTelltaleException ("Could not create backend stream");	
		}

		/// <summary>
		/// Determines whether this instance is little endian.
		/// </summary>
		/// <returns><c>true</c> if this instance is little endian; otherwise, <c>false</c>.</returns>
		public bool IsLittleEndian(){
			return Native.hByteOutStream_IsLittleEndian (reference) != 0;
		}

		/// <summary>
		/// Gets the backend buffer if this is not writing to a file. This is useful if you are writing to a byte[] and you want to get the output array.
		/// </summary>
		/// <returns>The buffer.</returns>
		public byte[] GetBuffer(){
			IntPtr ptr = Native.hByteOutStream_GetBuffer (reference);
			if (ptr.Equals (IntPtr.Zero))
				return null;
			byte[] ret = new byte[GetSize ()];
			Marshal.Copy (ptr,ret, 0, (int)GetSize ());
			//Dont want to free the buffer!!
			return ret;
		}

		/// <summary>
		/// Position to the specified offset.
		/// </summary>
		/// <param name="offset">Offset.</param>
		public void Position(ulong offset){
			Native.hByteOutStream_Position (reference, offset);
		}

		/// <summary>
		/// Sets the endian.
		/// </summary>
		public void SetEndian(bool little){
			Native.hByteOutStream_SetEndian (reference, little);
		}

		/// <summary>
		/// Gets the position.
		/// </summary>
		/// <returns>The position.</returns>
		public ulong GetPosition(){
			return Native.hByteOutStream_GetPosition (reference);
		}

		public void Dispose(){
			Native.hByteOutStream_Delete (this.reference);
		}

		/// <summary>
		/// Gets the size.
		/// </summary>
		/// <returns>The size.</returns>
		public ulong GetSize(){
			return Native.hByteOutStream_GetSize (reference);
		}

	}

	/// <summary>
	/// The handle for .vers meta streamed files. These files hold data about seriailzed file formats, but old ones. Also known as serialized version infos / meta version info.
	/// </summary>
	public sealed class MetaStreamedFile_Vers : IMetaStreamedFile {
		protected IntPtr reference;
		protected TTContext ctx;
		protected _Vers vers;

		/// <summary>
		/// The unmanaged memory structure of a vers block and its sub blocks.
		/// </summary>
		public struct _Vers {
			[MarshalAs(UnmanagedType.LPStr)]
			public string mTypeName;
			[MarshalAs(UnmanagedType.LPStr)]
			public string mFullTypeName;//If this is a MSV5+ .vers, then this is the fully qualified name of it. Else its the same as mTypeName
			public byte mbBlocked;
			public uint mBlockLengh;
			public uint mVersion;
			public IntPtr mBlockVarNames;
			public IntPtr mBlocks;
			public IntPtr ctx;
		}

		/// <summary>
		/// Creates an instance from the given context and ptr (internal)
		/// </summary>
		public MetaStreamedFile_Vers(TTContext ctx,IntPtr vers){
			this.reference = vers;
			this.ctx = ctx;
			UpdateAndSync (true);
		}

		/// <summary>
		/// Creates an instance from the given context
		/// </summary>
		public MetaStreamedFile_Vers(TTContext ctx){
			this.ctx = ctx;
			this.reference = Native.Vers_Create (ctx.Internal_Get());
			UpdateAndSync (true);
		}
			
		public bool Equals(MetaStreamedFile_Vers other){
			return other.vers.mVersion == vers.mVersion && String.Equals (vers.mTypeName, other.vers.mTypeName);
		}

		/// <summary>
		/// Gets the name of this .vers main blocks type name
		/// </summary>
		/// <returns>The version type name.</returns>
		public string GetVersionTypeName(){
			return this.vers.mTypeName;
		}

		/// <summary>
		/// Gets the length of this main vers block in bytes when loaded in memory
		/// </summary>
		/// <returns>The block length.</returns>
		public uint GetBlockLength(){
			return this.vers.mBlockLengh;
		}

		/// <summary>
		/// Gets a sub block by its index (use get count, for iteration)
		/// </summary>
		public _Vers GetVersBlock(int index){
			return (_Vers)Marshal.PtrToStructure (Native.VersBlocks_DCArray_At (this.vers.mBlocks, index), typeof(_Vers));
		}

		/// <summary>
		/// Gets the name of a sub blocks variable name. 
		/// </summary>
		/// <returns>The variable name.</returns>
		public string GetVarName(int index){
			if (this.vers.mBlockVarNames.Equals (IntPtr.Zero))
				return "";
			IntPtr ptr = this.vers.mBlockVarNames;
			ptr = new IntPtr (Marshal.ReadInt64 (new IntPtr(ptr.ToInt64() + (8*index))));
			return Marshal.PtrToStringAnsi (ptr);
		}

		/// <summary>
		/// Gets the inner (sub) block count.
		/// </summary>
		public int GetInnerBlockCount(){
			return Native.VersBlocks_DCArray_Size (this.vers.mBlocks);
		}

		/// <summary>
		/// If this vers block is blocked. Not sure on what this means
		/// </summary>
		public bool IsBlocked(){
			return this.vers.mbBlocked != 0;
		}

		/// <summary>
		/// Gets the versio of this .vers.
		/// </summary>
		/// <returns>The version.</returns>
		public uint GetVersion(){
			return this.vers.mVersion;
		}

		/// <summary>
		/// Open from the current context
		/// </summary>
		public int Open(){
			int i = Native.Vers_Open (this.reference);
			if (i == Config.OPEN_OK)
				UpdateAndSync (true);
			return i;
		}

		/// <summary>
		/// Writes this instance to the current context
		/// </summary>
		public bool Flush(){
			bool r = Native.Vers_Flush (this.reference, ctx.Internal_Get ()) != 0;
			if (r)
				UpdateAndSync (true);
			return r;
		}

		/// <summary>
		/// Frees all backend memory from this serialized version info 
		/// </summary>
		public void Dispose(){
			Native.Vers_Free (this.reference);
		}

		public IntPtr Internal_Get(){
			return this.reference;
		}

		public TTContext GetContext(){
			return this.ctx;
		}

		protected void UpdateAndSync(bool retrieve){
			if (retrieve) {
				this.vers = (_Vers)Marshal.PtrToStructure (this.reference, typeof(_Vers));
			} else {
				Marshal.StructureToPtr (this.vers, this.reference, false);
			}
		}

	}

	/// <summary>
	/// A meta class description in memory
	/// </summary>
	public struct _MetaClassDesc {
		[MarshalAs(UnmanagedType.LPStr)]
		public string mTypeName;
		public uint mVersion;//The version isnt actually a number, its some sort of CRCed string which I haven't got to yet. Version '0' for ints and booleans was the crc of 0 in memory (0,0,0,0) so im guessing its a 
		//string length since I tried all combinations of numbers from 0 - 0xFFFFFFFF. However until I find the string values for it, its going to be a uint32.
		public ulong mTypeNameCrc;
		public uint mVersionCrc;//this is for now the same as the mversion until i find out the above. the library will crc it you dont have to do that. although this is the same as the version (unless v0, then mVersoin is 0)
	}

	/// <summary>
	/// A meta class description as described in any meta streamed file meta stream header.
	/// </summary>
	public struct MetaClassDescription {
		public _MetaClassDesc backend;
		public IntPtr reference;
	}

	/// <summary>
	/// Represents the meta stream header in any meta streamed file. This is the header you see in most telltale files (5VSM, 6VSM, ERTM, NIBM, SEBM). MOCM,4VSM are not supported (they arent released).
	/// </summary>
	public sealed class MetaStream : IDisposable {

		/// <summary>
		/// Use this to create a meta class description entry for a meta stream. 
		/// </summary>
		public static MetaClassDescription CreateMetaClass(string typeName, uint version){
			IntPtr ptr = Marshal.AllocHGlobal (Marshal.SizeOf(typeof(_MetaClassDesc)));
			Marshal.Copy (new byte[Marshal.SizeOf (typeof(_MetaClassDesc))], 0, ptr, Marshal.SizeOf (typeof(_MetaClassDesc)));
			MetaClassDescription desc = new MetaClassDescription ();
			desc.backend = new _MetaClassDesc ();
			desc.backend.mTypeName = typeName;
			desc.backend.mVersion = version;
			desc.reference = ptr;
			Marshal.StructureToPtr (desc.backend, ptr, false);
			return desc;
		}

		protected IntPtr reference;

		public IntPtr Internal_Get(){
			return this.reference;
		}
	
		public MetaStream(IntPtr r) {
			reference = r;
		}

		/// <summary>
		/// Create a new meta stream
		/// </summary>
		public MetaStream(){
			this.reference = Native.hMetaStream_Create ();
		}

		/// <summary>
		/// Removes a meta class description entry
		/// </summary>
		public void RemoveClass(MetaClassDescription desc){
			Native.MetaStreamClasses_DCArray_Remove (this._GClasses (), desc.reference);
		}

		/// <summary>
		/// Adds a meta class description entry
		/// </summary>
		public void AddClass(MetaClassDescription desc){
			Native.MetaStreamClasses_DCArray_Add (this._GClasses (), desc.reference);
		}

		/// <summary>
		/// The amount of meta class description entries in this meta stream
		/// </summary>
		public int GetClasses(){
			return Native.MetaStreamClasses_DCArray_Size (this._GClasses ());
		}

		/// <summary>
		/// Similar to AddClass but creates it with the given name and version.
		/// </summary>
		public void AddNewClass(string typeName, uint version){
			this.AddClass (CreateMetaClass (typeName, version));
		}

		/// <summary>
		/// Opens this meta stream from the given input stream of bytes
		/// </summary>
		public bool Open(ByteStream stream){
			return Native.hMetaStream_Open (this.reference, stream.reference) != 0;
		}

		/// <summary>
		/// Writes this meta stream to the given output stream of bytes
		/// </summary>
		public void Flush(ByteOutStream stream){
			Native.hMetaStream_Flush (this.reference, stream.reference);
		}

		/// <summary>
		/// Gets the version of this meta stream (will be one of Config.META_x)
		/// </summary>
		public uint GetVersion(){
			return Native.hMetaStream_GetVersion (this.reference);
		}

		/// <summary>
		/// GetVersion() as a string (eg returns MSV6)
		/// </summary>
		public string GetVersionAsString(){
			byte[] str = BitConverter.GetBytes (GetVersion ());
			Array.Reverse (str);
			return System.Text.ASCIIEncoding.Default.GetString (str);
		}

		/// <summary>
		/// Gets the flags of this meta stream
		/// </summary>
		public uint GetFlags(){
			return Native.hMetaStream_GetFlags(this.reference);
		}

		/// <summary>
		/// Gets the size of the file this meta stream describes. Automatically set by TTContext on flush!
		/// </summary>
		public uint GetPayloadSize(){
			return Native.hMetaStream_GetPayloadSize (this.reference);
		}

		/// <summary>
		/// If this is a texture (or .bundle? not got to it) then this is the size of the raw texture data
		/// </summary>
		public uint GetTextureSize(){
			return Native.hMetaStream_GetTextureSize (this.reference);
		}

		/// <summary>
		/// Gets the class version of a given type name (searches for a meta class description entry with the type name and returns its version)
		/// </summary>
		public uint GetClassVersion(string typeName){
			return Native.hMetaStream_GetClassVersion (this.reference, typeName, 0);
		}

		/// <summary>
		/// Determines whether this instance has a class of the specified type name.
		/// </summary>
		public bool HasClass(string typeName){
			return GetClassVersion (typeName) != 0;
		}

		/// <summary>
		/// Closes this meta stream. This resets all inner class descriptions, the version,flags,sizes etc all to 0. Ready for the next open.
		/// </summary>
		public void Close(){
			Native.hMetaStream_Close (this.reference);
		}

		/// <summary>
		/// Clears the meta class description classes
		/// </summary>
		public void ClearClasses(){
			Native.MetaStreamClasses_DCArray_Clear (this._GClasses ());
		}

		/// <summary>
		/// Gets a meta class at the specified index (used for iteration)
		/// </summary>
		public MetaClassDescription GetMetaClass(int index){
			MetaClassDescription desc = new MetaClassDescription ();
			desc.backend = (_MetaClassDesc)Marshal.PtrToStructure (desc.reference = Native.MetaStreamClasses_DCArray_At(this._GClasses(), index), typeof(_MetaClassDesc));
			return desc;
		}

		protected IntPtr _GClasses(){
			return Native.hMetaStream_GetClasses (this.reference);
		}

		/// <summary>
		/// Sets the meta version. Must be one of the Config.META_x
		/// </summary>
		public void SetMetaVersion(uint Version){
			if (!(Version == Config.META_BIN || Version == Config.META_TRE || Version == Config.META_V5 || Version == Config.META_V6))
				return;
			Native.hMetaStream_SetVersion (this.reference, Version);
		}

		/// <summary>
		/// Deletes the backend memory of this. Suggested not to use this, just let the TTContext handle it.
		/// </summary>
		public void Dispose(){
			Native.hMetaStream_Delete (this.reference);
			this.reference = IntPtr.Zero;
		}

		/// <summary>
		/// Sets the flags for this meta stream, useful for ORing. 
		/// </summary>
		/// <param name="flags">Flags.</param>
		public void SetFlags(uint flags){
			Native.hMetaStream_SetFlags (reference, flags);
		}

	}

	/// <summary>
	/// Telltale file reading (and writing, see writingcontext) context. Use it when reading or writing any files or archives, and you are advised not to make too many of these objects.
	/// </summary>
	public sealed class TTContext : IDisposable {
		protected IntPtr reference;

		/// <summary>
		/// Deletes the backend memory of this context. This deletes the current stream and out stream, so be careful.
		/// </summary>
		public void Dispose(){
			Native.hTTContext_Delete (this.reference);
			this.reference = IntPtr.Zero;
		}

		/// <summary>
		/// Creates a context which will search files from the given archive.
		/// </summary>
		public TTContext(TTArchives.TTArchive archive, string gameID){
			IntPtr str1 = Marshal.StringToHGlobalAnsi (gameID);
			IntPtr ptr = Native.hMemory_CreateArray (str1);
			if (archive == null) {
				this.reference = Native.hTTContext_Create ( ptr, IntPtr.Zero);
			} else {
				IntPtr ptr2 = Native.hTTArchiveOrTTArchive2_Create ();
				Marshal.StructureToPtr (TTArchives.TTArchiveOrTTArchive2.CreateFromArchive (archive), ptr2, false);
				this.reference = Native.hTTContext_Create (ptr, ptr2);
			}
			Native.hMemory_FreeArray (ptr);
			Marshal.FreeHGlobal (str1);
		}

		/// <summary>
		/// Creates a context which will search files from the given archive.
		/// </summary>
		public TTContext(TTArchives.TTArchive2 archive, string gameID) {
			IntPtr str1 = Marshal.StringToHGlobalAnsi (gameID);
			IntPtr ptr = Native.hMemory_CreateArray (str1);
			if (archive == null) {
				this.reference = Native.hTTContext_Create ( ptr, IntPtr.Zero);
			} else {
				IntPtr ptr2 = Native.hTTArchiveOrTTArchive2_Create ();
				Marshal.StructureToPtr (TTArchives.TTArchiveOrTTArchive2.CreateFromArchive (archive), ptr2, false);
				this.reference = Native.hTTContext_Create (ptr, ptr2);
			}
			Native.hMemory_FreeArray (ptr);
			Marshal.FreeHGlobal (str1);
		}

		/// <summary>
		/// Creates a context which doesn't read from an archive. When reading meshes for example, textures may not be accessible since it requires the archive for the textures.
		/// </summary>
		public TTContext(string gameID) : this((TTArchives.TTArchive)null, gameID) {}

		/// <summary>
		/// Switches this context to the next archive. The del parameter specified if the previous meta/in and out stream should be deleted and disposed if they exist.
		/// NOTE: This does NOT delete the previous archive! This is because they are big and alot of the time you want to keep them open.
		/// </summary>
		public void NextArchive(TTArchives.TTArchive archive, bool del){
			IntPtr ptr = Native.hTTArchiveOrTTArchive2_Create ();
			Marshal.StructureToPtr (TTArchives.TTArchiveOrTTArchive2.CreateFromArchive (archive), ptr, false);
			Native.hTTContext_NextArchive (this.reference, ptr, del);
		}

		/// <summary>
		/// Switches this context to the next archive. The del parameter specified if the previous meta/in and out stream should be deleted and disposed if they exist.
		/// NOTE: This does NOT delete the previous archive! This is because they are big and alot of the time you want to keep them open.
		/// </summary>
		public void NextArchive(TTArchives.TTArchive2 archive, bool del){
			IntPtr ptr = Native.hTTArchiveOrTTArchive2_Create ();
			Marshal.StructureToPtr (TTArchives.TTArchiveOrTTArchive2.CreateFromArchive (archive), ptr, false);
			Native.hTTContext_NextArchive (this.reference, ptr, del);
		}

		/// <summary>
		/// Finds an entry (returning its name) in the current archive by its crc64 of its file name. Used mostly internally, but can be useful.
		/// Returns an empty string if there is no current archive.
		/// </summary>
		public string FindArchiveEntry(ulong crc64){
			IntPtr str = Native.hTTContext_FindArchiveEntry (this.reference, crc64);
			if (str.Equals (IntPtr.Zero))
				return "";
			return Marshal.PtrToStringAnsi (str);
		}

		/// <summary>
		/// If a previous NextRead has been set, then this is the start offset of the file after the meta header.
		/// </summary>
		public uint GetFileStart(){
			return Native.hTTContext_FileStart (this.reference);
		}

		/// <summary>
		/// Gets the meta stream header of the current reading/writing file in this context.
		/// </summary>
		/// <returns>The current meta.</returns>
		public MetaStream GetCurrentMeta(){
			IntPtr ptr = Native.hTTContext_CurrentMeta (this.reference);
			if(ptr.Equals(IntPtr.Zero))
				return null;
			return new MetaStream (ptr);
		}

		/// <summary>
		/// Overrides the current meta stream, deleting the meta stream in memory with the del parameter. Useful when writing a file, but you haven't read a file first to set its meta.
		/// </summary>
		public void OverrideMetaStream(MetaStream stream,bool del){
			Native.hTTContext_OverrideMeta(this.reference, stream.Internal_Get(), del);
		}
			
		/// <summary>
		/// Finalizes the current write after NextWrite and wanted file's Flush().
		/// The second parameter specifies if the entry in the archive's stream should be updated with this new one (you can forget about the byteoutstream object, it will be handled). This makes it easier
		/// to edit archives. If there is no archive attached then this does nothing. The delete parameter is still important however, as the byte output stream writing to will still be disposed of
		/// because the backend bytes of the file are copied to a bytestream since an archive stream is a readable one. So this means the last writing stream will be deleted, just not the bytes of it.
		/// The entry which this sets the stream for is the entry in the attached archive with the name passed in the NextWrite method which should have been previously called. If the entry doesn't exist
		/// this also does nothing too, so make sure you have at least created the entry.
		/// </summary>
		public void FinishCurrentWrite(bool del, bool updatearc){
			Native.hTTContext_FinishWrite (this.reference, del,updatearc);
		}

		/// <summary>
		/// Gets name (or an empty string if not writing) of the current file which is being written.
		/// </summary>
		public string GetCurrentWritingFile(){
			IntPtr nptr = Native.hTTContext_CurrentFile (this.reference);
			if (nptr.Equals (IntPtr.Zero))
				return "";
			return Marshal.PtrToStringAnsi (nptr);
		}

		/// <summary>
		/// Updates and initializes this context to the new writing stream. This doesn't affect the reading streams, but does reset and delete if del the previous outstream.
		/// You are required to pass the full file name of the writing file, this has to be the EXACT file name with extension and casing! If you read it, has to be the same as
		/// it was before. This requires a meta stream to be set by next read or override current meta, or it wont work. The stream parameter can be a direct new ByteOutStream, since it only needs
		/// to be handled by the library and only will be. See FinishedCurrentWrite which explains what happens after you have called Flush on the file type object (eg InputMapper.Flush()).
		/// The stream parameter can be null, and if it is the out stream will be handled internally. This is useful if you are planning to write to back to a telltale archive. (Using update = true
		/// on the FinishCurrentWrite).
		/// </summary>
		public void NextWrite(ByteOutStream stream, string fileName,bool del){
			if (stream != null) {
				Native.hTTContext_NextWrite (this.reference, stream.reference, fileName, del);
			} else {
				Native.hTTContext_NextWrite (this.reference, Native.hByteOutStream_Create(0), fileName, del);
			}
		}

		/// <summary>
		/// Gets the current output stream this context is writing to.
		/// </summary>
		public ByteOutStream GetCurrentOutStream(){
			return new ByteOutStream (Native.hTTContext_CurrentOutStream (this.reference));
		}

		/// <summary>
		/// Gets the current stream.
		/// </summary>
		/// <returns>The current stream.</returns>
		public ByteStream GetCurrentStream(){
			IntPtr ptr = Native.hTTContext_CurrentStream (this.reference);
			if(ptr.Equals(IntPtr.Zero))
				return null;
			return new ByteStream (ptr);
		}

		/// <summary>
		/// Opens a stream from the backend archive. Bascially TTArchive<2> open stream.
		/// </summary>
		public ByteStream OpenArchiveStream(string archiveEntryName){
			IntPtr ptr = Native.hTTContext_OpenStream (this.reference,archiveEntryName);
			if(ptr.Equals(IntPtr.Zero))
				return null;
			return new ByteStream (ptr);
		}

		/// <summary>
		/// Updates this context to the new reading stream. This does read the meta stream but not the file (since you need it specific).
		/// If del is true, this deletes the old reading streams backend memory so if you have a reference to it still then be careful (DO NOT call its dispose!) or set this to false!
		/// </summary>
		public bool NextStream(ByteStream stream, bool del){
			return Native.hTTContext_NextStream (this.reference, stream.reference, del) != 0;
		}

		public IntPtr Internal_Get () {
			return reference;
		}

	}

	/// <summary>
	/// Base for all meta streamed files, excluding .vers (serialized version info; they specify formats), and all implementing classes derive from this interface.
	/// </summary>
	public interface IMetaStreamedFile : IDisposable {

		/// <summary>
		/// Opens after NextStream has been called on the current context. Returns one of the constants in config, where OPEN_OK (0) is successfull.
		/// </summary>
		int Open ();

		/// <summary>
		/// Writes this meta streamed file to the attached context, returning if it was successfull. Must be called after NextWrite in a TTContext, followed by FinishCurrentWrite.
		/// </summary>
		bool Flush();

		/// <summary>
		/// Used internally to get the backend pointer in memory to this object.
		/// </summary>
		IntPtr Internal_Get();

		/// <summary>
		/// Gets the attached context this meta streamed file uses.
		/// </summary>
		TTContext GetContext();

	}

	/// <summary>
	/// Represents an Input Mapper (.imap). These files map key bindings to lua script functions for different scenes in telltale episodes.
	/// </summary>
	public sealed class MetaStreamedFile_InputMapper : IMetaStreamedFile {

		/// <summary>
		/// All input codes currently known and supported. If there are any which you find that cause errors report on the github. 
		/// </summary>
		public enum InputCode {
			BACKSPACE = 8,
			NUM_0 = 48,
			NUM_1 = 49,
			NUM_2 = 50,
			NUM_3 = 51,
			NUM_4 = 52,
			NUM_5 = 53,
			NUM_6 = 54,
			NUM_7 = 55,
			NUM_8 = 56,
			NUM_9 = 57,
			BUTTON_A = 512,
			BUTTON_B = 513,
			BUTTON_X = 514,
			BUTTON_Y = 515,
			BUTTON_L = 516,
			BUTTON_R = 517,
			BUTTON_BACK = 518,
			BUTTON_START = 519,
			TRIGGER_L = 520,
			TRIGGER_R = 521,
			DPAD_UP = 524,
			DPAD_DOWN = 525,
			DPAD_RIGHT = 526,
			DPAD_LEFT = 527,
			CONFIRM = 768,
			CANCEL = 769,
			MOUSE_MIDDLE = 770,
			MOUSE_LEFT_DOUBLE = 772,
			MOUSE_MOVE = 784,
			ROLLOVER = 800,
			WHEEL_UP = 801,
			WHEEL_DOWN = 802,
			MOUSE_LEFT = 816,
			MOUSE_RIGHT = 817,
			LEFT_STICK = 1024,
			RIGHT_STICK = 1025,
			TRIGGER = 1026,
			TRIGGER_LEFT_MOVE = 1027,
			TRIGGER_RIGHT_MOVE = 1028,
			SWIPE_LEFT = 1296,
			SWIPE_RIGHT = 1298,
			DOUBLE_TAP_SCREEN = 1304,
			SHIFT = 16,
			ENTER = 13,
			ESCAPE = 27,
			LEFT_ARROW = 37,
			RIGHT_ARROW = 39,
			DOWN_ARROW = 40,
			UP_ARROW = 38,
			KEY_A = 65,
			KEY_B = 66,
			KEY_C = 67,
			KEY_D = 68,
			KEY_E = 69,
			KEY_F = 70,
			KEY_G = 71,
			KEY_H = 72,
			KEY_I = 73,
			KEY_J = 74,
			KEY_K = 75,
			KEY_L = 76,
			KEY_M = 77,
			KEY_N = 78,
			KEY_O = 79,
			KEY_P = 80,
			KEY_Q = 81,
			KEY_R = 82,
			KEY_S = 83,
			KEY_T = 84,
			KEY_U = 85,
			KEY_V = 86,
			KEY_W = 87,
			KEY_X = 88,
			KEY_Y = 89,
			KEY_Z = 90,
		};

		/// <summary>
		/// An event mapping event. This says if the event is when a key is pressed/clicked (BEGIN), or whether its when its released (END).
		/// </summary>
		public enum Event {
			BEGIN = 0,
			END = 1
		};

		/// <summary>
		/// An event mapping, which are all contained in a backend list vector of an InputMapper. See the mMapping field for the backend structure in memory, which contains the values. Use
		/// the static methods to set the function names.
		/// </summary>
		public struct EventMapping {
			public _EventMapping mMapping;
			public IntPtr reference;
		}

		/// <summary>
		/// The backend event mapping in memory. Do not edit these name (use static methods in the input mapper class). If you edit the fields, once done call the static 
		/// method UpdateEventMapping to update it in memory for when you rewrite it.
		/// </summary>
		public struct _EventMapping {
			public InputCode mInputCode;
			public Event mEvent;
			public IntPtr mScriptFunction;
			public int mControllerIndexOverride;
		}

		/// <summary>
		/// Updates the event mapping after you have edited any of the fields in it (apart from set script function).
		/// </summary>
		public static void UpdateEventMapping(EventMapping mapping){
			Marshal.StructureToPtr (mapping.mMapping, mapping.reference, false);
		}

		/// <summary>
		/// Gets the script function as a string from the given event mapping.
		/// </summary>
		public static string GetScriptFunction(EventMapping mapping){
			return Marshal.PtrToStringAnsi (mapping.mMapping.mScriptFunction);
		}

		/// <summary>
		/// Sets the script function for the given mapping.
		/// </summary>
		public static void SetScriptFunction(EventMapping mapping, string func){
			if (!mapping.mMapping.mScriptFunction.Equals (IntPtr.Zero)) {
				Native.hMemory_FreeArray (mapping.mMapping.mScriptFunction);
			}
			IntPtr ptr = Marshal.StringToHGlobalAnsi (func);
			mapping.mMapping.mScriptFunction = Native.hMemory_CreateArray (ptr);
			Marshal.FreeHGlobal (ptr);
		}

		/// <summary>
		/// Creates an event mapping with all of the data as the parameters.
		/// </summary>
		public static EventMapping CreateMapping(string function, InputCode inputCode, Event ev, int mControllerIndexOverride){
			EventMapping mapping = new EventMapping ();
			mapping.reference = Native.hInputMapping_CreateMapping ();
			mapping.mMapping = new _EventMapping ();
			mapping.mMapping.mScriptFunction = IntPtr.Zero;
			mapping.mMapping.mControllerIndexOverride = mControllerIndexOverride;
			mapping.mMapping.mEvent = ev;
			mapping.mMapping.mInputCode = inputCode;
			IntPtr ptr = Marshal.StringToHGlobalAnsi (function);
			mapping.mMapping.mScriptFunction = Native.hMemory_CreateArray (ptr);
			Marshal.FreeHGlobal (ptr);
			Marshal.StructureToPtr (mapping.mMapping, mapping.reference, false);
			return mapping;
		}

		protected IntPtr reference;

		protected TTContext context;

		/// <summary>
		/// Creates a new input mapper, ready to be edited and opened using Open.
		/// </summary>
		public MetaStreamedFile_InputMapper(TTContext context){
			this.reference = Native.hInputMapper_Create ();
			this.context = context;
		}
			
		public TTContext GetContext(){
			return this.context;
		}

		public IntPtr Internal_Get(){
			return this.reference;
		}

		public bool Flush(){
			return Native.InputMapper_Flush (context.Internal_Get (), this.reference);
		}

		public int Open(){
			return Native.InputMapper_Open (context.Internal_Get (), this.reference);
		}

		/// <summary>
		/// Deletes this Input Mapper and all backend memory associated.
		/// </summary>
		public void Dispose(){
			Native.hInputMapper_Delete (this.reference);
		}
			
		protected IntPtr Mappings(){
			return Native.hInputMapper_Mappings (this.reference);
		}

		/// <summary>
		/// Gets the amount of mappings in this input mapper.
		/// </summary>
		public uint GetMappings(){
			return (uint)Native.InputMapper_DCArray_Size (this.Mappings());
		}

		/// <summary>
		/// Clears the mappings of this input mapper.
		/// </summary>
		public void ClearMappings(){
			Native.InputMapper_DCArray_Clear (this.Mappings ());
		}

		/// <summary>
		/// Used for iteration to get the mapping at the specified index.
		/// </summary>
		public EventMapping GetMapping(uint index){
			EventMapping mapping = new EventMapping ();
			mapping.reference = Native.InputMapper_DCArray_At (this.Mappings (), (int)index);
			mapping.mMapping = ( _EventMapping)Marshal.PtrToStructure (mapping.reference, typeof(_EventMapping));
			return mapping;
		}

		/// <summary>
		/// Adds a new mapping.
		/// </summary>
		public void AddMapping(EventMapping mapping){
			Native.InputMapper_DCArray_Add (this.Mappings(), mapping.reference);
		}

		/// <summary>
		/// Removes a mapping.
		/// </summary>
		public void RemoveMapping(EventMapping mapping){
			Native.InputMapper_DCArray_Remove (this.Mappings(), mapping.reference);
		}

	}

	/// <summary>
	/// Native access to the telltale library DLL. This is a private class because it is used internally.
	/// </summary>
	class Native {

		[DllImport("LibTelltale.dll")]
		public static extern void hTTArchiveOrTTArchive2_Delete(IntPtr ptr);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hTTArchiveOrTTArchive2_Create();

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hInputMapping_CreateMapping();

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hMemory_CreateArray(IntPtr s);

		[DllImport("LibTelltale.dll")]
		public static extern int InputMapper_DCArray_Size (IntPtr ptr);

		[DllImport("LibTelltale.dll")]
		public static extern void hMemory_FreeArray (IntPtr ptr);

		[DllImport("LibTelltale.dll")]
		public static extern void InputMapper_DCArray_Clear (IntPtr ptr);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr InputMapper_DCArray_At(IntPtr ptr, int index);

		[DllImport("LibTelltale.dll")]
		public static extern void InputMapper_DCArray_Add (IntPtr ptr, IntPtr desc_);

		[DllImport("LibTelltale.dll")]
		public static extern void InputMapper_DCArray_Remove (IntPtr ptr, IntPtr desc_);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hInputMapper_Mappings (IntPtr ptr);

		[DllImport("LibTelltale.dll")]
		public static extern void hInputMapper_Delete(IntPtr imap);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hInputMapper_Create();

		[DllImport("LibTelltale.dll")]
		public static extern bool InputMapper_Flush (IntPtr ctx, IntPtr imap);

		[DllImport("LibTelltale.dll")]
		public static extern int InputMapper_Open (IntPtr ctx, IntPtr imap);

		[DllImport("LibTelltale.dll")]
		public static extern int MetaStreamClasses_DCArray_Size (IntPtr ptr);

		[DllImport("LibTelltale.dll")]
		public static extern void MetaStreamClasses_DCArray_Clear (IntPtr ptr);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr MetaStreamClasses_DCArray_At(IntPtr ptr, int index);

		[DllImport("LibTelltale.dll")]
		public static extern void MetaStreamClasses_DCArray_Add (IntPtr ptr, IntPtr desc_);

		[DllImport("LibTelltale.dll")]
		public static extern void MetaStreamClasses_DCArray_Remove (IntPtr ptr, IntPtr desc_);

		[DllImport("LibTelltale.dll")]
		public static extern int VersBlocks_DCArray_Size (IntPtr dcarray);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr VersBlocks_DCArray_At(IntPtr dcarray, int index);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hTTContext_Create(IntPtr archive, IntPtr f);

		[DllImport("LibTelltale.dll")]
		public static extern byte hTTContext_NextStream (IntPtr ctx, IntPtr strm, bool del);

		[DllImport("LibTelltale.dll")]
		public static extern uint hTTContext_FileStart (IntPtr strm);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hTTContext_CurrentStream (IntPtr strm);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hTTContext_CurrentMeta(IntPtr strm);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hTTContext_FindArchiveEntry(IntPtr ctx, ulong crc64);

		[DllImport("LibTelltale.dll")]
		public static extern void hTTContext_NextArchive (IntPtr prop, IntPtr archive, bool del);

		[DllImport("LibTelltale.dll")]
		public static extern byte hTTContext_NextWrite (IntPtr p, IntPtr strm, [MarshalAs (UnmanagedType.LPStr)] string fileName, bool del);

		[DllImport("LibTelltale.dll")]
		public static extern void hTTContext_OverrideMeta(IntPtr p, IntPtr meta, bool del);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hTTContext_CurrentOutStream(IntPtr ctx);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hTTContext_CurrentFile (IntPtr ctx);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hTTContext_OpenStream(IntPtr strm, [MarshalAs(UnmanagedType.LPStr)] string entryName);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hMetaStream_GetClasses (IntPtr ptr);

		[DllImport("LibTelltale.dll")]
		public static extern void hMetaStream_SetVersion(IntPtr stream, uint Version);

		[DllImport("LibTelltale.dll")]
		public static extern void hMetaStream_SetFlags (IntPtr stream, uint flags);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hMetaStream_Create ();

		[DllImport("LibTelltale.dll")]
		public static extern byte hMetaStream_Open (IntPtr meta, IntPtr stream);

		[DllImport("LibTelltale.dll")]
		public static extern void hMetaStream_Close (IntPtr meta);

		[DllImport("LibTelltale.dll")]
		public static extern uint hMetaStream_GetVersion (IntPtr m);

		[DllImport("LibTelltale.dll")]
		public static extern uint hMetaStream_GetFlags(IntPtr m);

		[DllImport("LibTelltale.dll")]
		public static extern uint hMetaStream_GetPayloadSize (IntPtr meta);

		[DllImport("LibTelltale.dll")]
		public static extern uint hMetaStream_GetTextureSize(IntPtr meta);

		[DllImport("LibTelltale.dll")]
		public static extern uint hMetaStream_GetClassVersion (IntPtr meta, [MarshalAs (UnmanagedType.LPStr)] string typeName, uint default_value);

		[DllImport("LibTelltale.dll")]
		public static extern void hTTContext_FinishWrite (IntPtr p, bool del, bool update);

		[DllImport("LibTelltale.dll")]
		public static extern void hTTContext_NextArchive (IntPtr ctx, IntPtr archive);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr Vers_Create (IntPtr ctx);

		[DllImport("LibTelltale.dll")]
		public static extern int Vers_Open (IntPtr vers);

		[DllImport("LibTelltale.dll")]
		public static extern byte Vers_Flush (IntPtr vers, IntPtr wctx);

		[DllImport("LibTelltale.dll")]
		public static extern void Vers_Free(IntPtr vers);

		[DllImport("LibTelltale.dll")]
		public static extern void hTTArchive_EntryRemove (IntPtr a, IntPtr b, bool c);

		[DllImport("LibTelltale.dll")]
		public static extern void hTTArchive2_EntryRemove (IntPtr a, IntPtr b, bool c);

		[DllImport("LibTelltale.dll")]
		public static extern void hTTArchive_EntrySetName (IntPtr a, [MarshalAs(UnmanagedType.LPStr)] string name);

		[DllImport("LibTelltale.dll")]
		public static extern int TTArchive2_Open (IntPtr a);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr TTArchive2_StreamOpen (IntPtr a, IntPtr b);

		[DllImport("LibTelltale.dll")]
		public static extern void TTArchive2_Free (IntPtr a);

		[DllImport("LibTelltale.dll")]
		public static extern int TTArchive2_Flush (IntPtr a,IntPtr b);

		[DllImport("LibTelltale.dll")]
		public static extern void TTArchive2_StreamSet (IntPtr a, IntPtr b);

		[DllImport("LibTelltale.dll")]
		public static extern void TTArchive2_EntrySetName (IntPtr a, [MarshalAs(UnmanagedType.LPStr)] string name);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr TTArchive2_EntryFind (IntPtr a, [MarshalAs(UnmanagedType.LPStr)] string name);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr TTArchive2_EntryCreate ([MarshalAs(UnmanagedType.LPStr)] string name, IntPtr b);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr TTArchive_StreamOpen(IntPtr a, IntPtr b);

		[DllImport("LibTelltale.dll")]
		public static extern void hTTContext_Delete(IntPtr ptr);

		[DllImport("LibTelltale.dll")]
		public static extern void hMetaStream_Delete(IntPtr ptr);

		[DllImport("LibTelltale.dll")]
		public static extern void hTTArchive_Delete (IntPtr ptr);

		[DllImport("LibTelltale.dll")]
		public static extern void hTTArchive2_Delete (IntPtr ptr);

		[DllImport("LibTelltale.dll")]
		public static extern void hByteStream_Delete(IntPtr ptr);

		[DllImport("LibTelltale.dll")]
		public static extern void hByteOutStream_Delete(IntPtr ptr);

		[DllImport("LibTelltale.dll")]
		public static extern int TTArchive_Open (IntPtr a);

		[DllImport("LibTelltale.dll")]
		public static extern void TTArchive_Free (IntPtr a);

		[DllImport("LibTelltale.dll")]
		public static extern int TTArchive_Flush(IntPtr a, IntPtr func);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr TTArchive_EntryCreate([MarshalAs(UnmanagedType.LPStr)] string name, IntPtr strm);

		[DllImport("LibTelltale.dll")]
		public static extern void TTArchive_StreamSet (IntPtr a, IntPtr b);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr TTArchive_EntryFind (IntPtr a, [MarshalAs (UnmanagedType.LPStr)] string name);

		[DllImport("LibTelltale.dll")]
		public static extern void hByteStream_Position(IntPtr stream, ulong off);

		[DllImport("LibTelltale.dll")]
		public static extern byte hByteStream_IsLittleEndian(IntPtr stream);

		[DllImport("LibTelltale.dll")]
		public static extern void hByteStream_SetEndian(IntPtr s, bool little_endian);

		[DllImport("LibTelltale.dll")]
		public static extern ulong hByteStream_ReadInt(IntPtr s, uint bitwidth);

		[DllImport("LibTelltale.dll")]
		public static extern ulong hByteStream_GetPosition(IntPtr s);

		[DllImport("LibTelltale.dll")]
		public static extern ulong hByteStream_GetSize(IntPtr s);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hByteStream_ReadBytes(IntPtr s, uint size);

		[DllImport("LibTelltale.dll")]
		public static extern void hTTArchive2_EntryAdd(IntPtr p, IntPtr b);

		[DllImport("LibTelltale.dll")]
		public static extern void hMetaStream_Flush (IntPtr c, IntPtr str);

		[DllImport("LibTelltale.dll")]
		public static extern void hTTArchive_EntryAdd(IntPtr p, IntPtr b);

		[DllImport("LibTelltale.dll")]
		public static extern byte hByteStream_ReadByte(IntPtr s);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hByteStream_Create (uint size);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hByteStream_CreateFromBuffer (IntPtr p, uint size);

		[DllImport("LibTelltale.dll") ]
		public static extern IntPtr hByteStream_ReadString(IntPtr s, uint len);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hByteStream_ReadString0(IntPtr s);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hFileStream_Create([MarshalAs(UnmanagedType.LPStr)] string filepath);

		[DllImport("LibTelltale.dll")]
		public static extern byte hByteStream_Valid(IntPtr s);

		[DllImport("LibTelltale.dll")]
		public static extern void hMemory_Free(IntPtr ptr);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hMemory_Alloc(uint size);

		[DllImport("LibTelltale.dll")]
		public static extern byte hByteOutStream_Valid(IntPtr s);

		[DllImport("LibTelltale.dll")]
		public static extern uint hTTArchive2_GetEntryCount(IntPtr archive);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hTTArchive2_GetEntryAt (IntPtr archive, uint index);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hTTArchive_GetEntryAt (IntPtr archive, uint index);

		[DllImport("LibTelltale.dll")]
		public static extern void hTTArchive2_ClearEntries (IntPtr archive);

		[DllImport("LibTelltale.dll")]
		public static extern void hByteOutStream_Position(IntPtr stream, ulong off);

		[DllImport("LibTelltale.dll")]
		public static extern byte hByteOutStream_IsLittleEndian(IntPtr stream);

		[DllImport("LibTelltale.dll")]
		public static extern void hByteOutStream_SetEndian(IntPtr s, bool little_endian);

		[DllImport("LibTelltale.dll")]
		public static extern void hByteOutStream_WriteInt (IntPtr s, uint bitwidth, ulong num);

		[DllImport("LibTelltale.dll")]
		public static extern ulong hByteOutStream_GetPosition(IntPtr s);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hByteOutStream_GetBuffer(IntPtr s);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hByteStream_GetBuffer(IntPtr s);

		[DllImport("LibTelltale.dll")]
		public static extern ulong hByteOutStream_GetSize(IntPtr s);

		[DllImport("LibTelltale.dll")]
		public static extern void hByteOutStream_WriteBytes(IntPtr s, IntPtr buf, uint size);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hByteOutStream_Create (uint size);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hFileOutStream_Create([MarshalAs(UnmanagedType.LPStr)] string filepath);

		[DllImport("LibTelltale.dll")]
		public static extern void hTTArchive_ClearEntries (IntPtr p);

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hTTArchive2_Create ();

		[DllImport("LibTelltale.dll")]
		public static extern IntPtr hTTArchive_Create ();

		[DllImport("LibTelltale.dll")]
		public static extern uint hTTArchive_GetEntryCount(IntPtr archive);

	}

}