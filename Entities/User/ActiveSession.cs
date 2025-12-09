using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.User
{
	public class ActiveSession : IEntity
	{
        public int Id { get; set; }
		public int? CreatorUserId { get; set; }

		[ForeignKey("CreatorUserId")]
		public ApplicationUser ApplicationUser { get; set; }

		public string CreatorIP { get; set; }

        public string DeviceName { get; set; }

		public DateTime LoginDate { get; set; }

        public OSType OSType { get; set; }

        public BrowserType BrowserType { get; set; }

		public DeviceType DeviceType { get; set; }

        public string Token { get; set; }
	}

    public class ActiveSessionConfiguration : IEntityTypeConfiguration<ActiveSession>
    {
        public void Configure(EntityTypeBuilder<ActiveSession> builder)
        {
  
            builder.HasOne(a=> a.ApplicationUser).WithMany(a=> a.ActiveSessions).OnDelete(DeleteBehavior.Cascade);  

        }
    }

    public enum BrowserType
	{
        Unknown,
        Firefox,
        Chrome,
        MobileSafari,
        Edge,
        ChromeWebView,
        Amaya,
        AndroidBrowser,
        Arora,
        Avant,
        Avast,
        AVG,
        BIDUBrowser,
        Baidu,
        Basilisk,
        Blazer,
        Bolt,
        Brave,
        Bowser,
        Camino,
        Chimera,
        ChromeHeadless,
     
       
        Chromium,
        Cobalt,
        ComodoDragon,
        Dillo,
        Dolphin,
        Doris,
        DuckDuckGo,
      
        Electron,
        Epiphany,
        Facebook,
        Falkon,
        Fennec,
        Firebird,
        FirefoxFocus,
        FirefoxReality,
        Flock,
        Flow,
        GSA,
        GoBrowser,
        Heytap,
        HuaweiBrowser,
        ICEBrowser,
        IE,
        IEMobile,
        IceApe,
        IceCat,
        IceDragon,
        Iceweasel,
        Instagram,
        Iridium,
        Iron,
        Jasmine,
        KakaoStory,
        KakaoTalk,
        KMeleon,
        Kindle,
        Klar,
        Konqueror,
        LBBROWSER,
        Line,
        LinkedIn,
        Links,
        Lunascape,
        Lynx,
        MIUIBrowser,
        Maemo,
        Maxthon,
        MetaSr,
        Midori,
        Minimo,
     
        Mosaic,
        Mozilla,
        NetFront,
        NetSurf,
        Netfront,
        Netscape,
        NokiaBrowser,
        Obigo,
        OculusBrowser,
        OmniWeb,
        OperaCoast,
        OperaMini,
        OperaMobi,
        OperaTablet,
        PaleMoon,
        PhantomJS,
        Phoenix,
        Polaris,
        Puffin,
        QQ,
        QQBrowser,
        QQBrowserLite,
        Quark,
        QupZilla,
        RockMelt,
        Safari,
        SailfishBrowser,
        SamsungBrowser,
        SeaMonkey,
        Silk,
        Skyfire,
        Sleipnir,
        Slim,
        SlimBrowser,
        Swiftfox,
        Tesla,
        TikTok,
        TizenBrowser,
        UCBrowser,
        UPBrowser,
        Viera,
        Vivaldi,
        Waterfox,
        WeChat,
        Weibo,
        Yandex,
        iCab,
        w3m,
        Whale
    }

	public enum DeviceType
	{
        Unknown,
        Desktop,
        Mobile,
        Tablet,
        Console,
        TV,
        Wearable,
        EReader,
    }

	public enum OSType
	{
        Unknown,
        Windows,
        Android,
        iOS,
        Linux,
        MacOS,
        AIX,
        AmigaOS,
       
        Androidx86,
        Arch,
        Bada,
        BeOS,
        BlackBerry,
        CentOS,
        ChromiumOS,
        Contiki,
        Fedora,
        FirefoxOS,
        FreeBSD,
        Debian,
        Deepin,
        DragonFly,
        elementaryOS,
        Fuchsia,
        Gentoo,
        GhostBSD,
        GNU,
        Haiku,
        HarmonyOS,
        HPUX,
        Hurd,
     
        Joli,
        KaiOS,
        Linpus,
        Linspire,
     
        Maemo,
        Mageia,
        Mandriva,
        Manjaro,
        MeeGo,
        Minix,
        Mint,
        MorphOS,
        NetBSD,
        NetRange,
        NetTV,
        Nintendo,
        OpenBSD,
        OpenVMS,
        OS2,
        Palm,
        PCBSD,
        PCLinuxOS,
        Plan9,
        PlayStation,
        QNX,
        Raspbian,
        RedHat,
        RIMTabletOS,
        RISCOS,
        Sabayon,
        Sailfish,
        SerenityOS,
        Series40,
        Slackware,
        Solaris,
        SUSE,
        Symbian,
        Tizen,
        Ubuntu,
        Unix,
        VectorLinux,
        Viera,
        watchOS,
        WebOS,
        WindowsPhone,
        WindowsMobile,
       
        Zenwalk
    }
}
