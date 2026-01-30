using QHackLib;
using QHackLib.Memory;
using QHackLib.Assemble;
using QHackLib.FunctionHelper;
using QTRHacker.Core;
using static QTRHacker.Scripts.ScriptHelper;
using System.Windows;

namespace QTRHacker.Scripts.Functions;
public class CreativeMenu : BaseFunction
{
	public override bool CanDisable => true;
	public override void ApplyLocalization(string culture)
	{
		switch (culture)
		{
			case "zh":
				Name = "旅行模式菜单";
				Tooltip = "在非旅行模式下可用";
				break;
			case "en":
			default:
				Name = "Journey Mode Menu";
				Tooltip = "Force journey mode menu enbaled";
				break;
		}
	}
	public override void Enable(GameContext ctx)
	{
		int off = GetOffset(ctx, "Terraria.Player", "difficulty");
		AobReplace(ctx, $"80 B8 {AobscanHelper.GetMByteCode(off)} 03 74", $"80 B8 {AobscanHelper.GetMByteCode(off)} 03 EB");
		IsEnabled = true;
	}
	public override void Disable(GameContext ctx)
	{
		int off = GetOffset(ctx, "Terraria.Player", "difficulty");
		AobReplace(ctx, $"80 B8 {AobscanHelper.GetMByteCode(off)} 03 EB", $"80 B8 {AobscanHelper.GetMByteCode(off)} 03 74");
		IsEnabled = false;
	}
}

public class UnlockAllDuplications : BaseFunction
{
	private string ErrorMsg1 { get; set; }
	public override bool CanDisable => false;
	public override void ApplyLocalization(string culture)
	{
		switch (culture)
		{
			case "zh":
				Name = "解锁所有研究";
				Tooltip = "旅行模式菜单";
				ErrorMsg1 = "请先关闭/折叠旅行模式菜单";
				break;
			case "en":
			default:
				Name = "Unlock all duplications";
				Tooltip = "In journey mode menu";
				ErrorMsg1 = "Please first close or fold journey mode menu";
				break;
		}
	}
	public override void Enable(GameContext ctx)
	{
		dynamic enabled = ctx.GameModuleHelper
			.GetStaticHackObject("Terraria.Main", "CreativeMenu")
			.InternalGetMember("<Enabled>k__BackingField");
		if ((bool)enabled)// This restriction prevents crashes
		{
			MessageBox.Show(ErrorMsg1);
			return;
		}
		HackObject c = ctx.MyPlayer.InternalObject.creativeTracker.ItemSacrifices;
		nuint addr = ctx.GameModuleHelper
			.GetFunctionAddress("Terraria.GameContent.Creative.ItemsSacrificedUnlocksTracker",
			"RegisterItemSacrifice");
		var code = AssemblySnippet.FromCode(new AssemblyCode[] {
				AssemblySnippet.Loop(
					AssemblySnippet.FromCode(new AssemblyCode[] {
						(Instruction)$"mov ecx, {c.BaseAddress}",
						(Instruction)$"mov edx, [esp]",
						(Instruction)$"push 9999",
						(Instruction)$"call {addr}",
					}),
					GameConstants.MaxItemTypes, true)
			});
		var task = Task.Run(() => ctx.RunOnManagedThread(code).WaitToDispose());
		if (!task.Wait(2000))
		{
			//TODO: abort the task
		}
	}
	public override void Disable(GameContext ctx)
	{
		throw new InvalidOperationException();
	}
}

public class InfiniteLife : BaseFunction
{
	public override bool CanDisable => true;
	public override void ApplyLocalization(string culture)
	{
		switch (culture)
		{
			case "zh":
				Name = "无限生命";
				Tooltip = "免疫大部分伤害";
				break;
			case "en":
			default:
				Name = "Infinite Life";
				Tooltip = "Immune to most damages except continuous ones like burning";
				break;
		}
	}
	public override void Enable(GameContext ctx)
	{
		int off = GetOffset(ctx, "Terraria.Player", "statLife");
		AobReplace(ctx, $"29 82 {AobscanHelper.GetMByteCode(off)} 83 7D", $"01 82 {AobscanHelper.GetMByteCode(off)} 83 7D");
		IsEnabled = true;
	}
	public override void Disable(GameContext ctx)
	{
		int off = GetOffset(ctx, "Terraria.Player", "statLife");
		AobReplace(ctx, $"01 82 {AobscanHelper.GetMByteCode(off)} 83 7D", $"29 82 {AobscanHelper.GetMByteCode(off)} 83 7D");
		IsEnabled = false;
	}
}

public class InfiniteMana : BaseFunction
{
	public override bool CanDisable => true;
	public override void ApplyLocalization(string culture)
	{
		Name = culture switch
		{
			"zh" => "无限魔法",
			_ => "Infinite Mana",
		};
	}
	public override void Enable(GameContext ctx)
	{
		int off = GetOffset(ctx, "Terraria.Player", "statMana");
		AobReplaceASM(ctx, $"sub [esi+{off}],edi", $"add [esi+{off}],edi");
		AobReplaceASM(ctx, $"sub [esi+{off}],eax", $"add [esi+{off}],eax");
		IsEnabled = true;
	}
	public override void Disable(GameContext ctx)
	{
		int off = GetOffset(ctx, "Terraria.Player", "statMana");
		AobReplaceASM(ctx, $"add [esi+{off}],edi", $"sub [esi+{off}],edi");
		AobReplaceASM(ctx, $"add [esi+{off}],eax", $"sub [esi+{off}],eax");
		IsEnabled = false;
	}
}

public class InfiniteOxygen : BaseFunction
{
	public override bool CanDisable => true;
	public override void ApplyLocalization(string culture)
	{
		Name = culture switch
		{
			"zh" => "无限氧气",
			_ => "Infinite Oxygen",
		};
	}
	public override void Enable(GameContext ctx)
	{
		int off = GetOffset(ctx, "Terraria.Player", "breath");
		AobReplaceASM(ctx, $"dec dword ptr [eax+{off}]\ncmp dword ptr [eax+{off}],0", $"inc dword ptr [eax+{off}]");
		IsEnabled = true;
	}
	public override void Disable(GameContext ctx)
	{
		int off = GetOffset(ctx, "Terraria.Player", "breath");
		AobReplaceASM(ctx, $"inc dword ptr [eax+{off}]\ncmp dword ptr [eax+{off}],0", $"dec dword ptr [eax+{off}]");
		IsEnabled = false;
	}
}

public class InfiniteMinion : BaseFunction
{
	public override bool CanDisable => true;
	public override void ApplyLocalization(string culture)
	{
		Name = culture switch
		{
			"zh" => "无限召唤物",
			_ => "Infinite Minion",
		};
	}
	public override void Enable(GameContext ctx)
	{
		int offA = GetOffset(ctx, "Terraria.Player", "maxMinions");
		int offB = GetOffset(ctx, "Terraria.Player", "maxTurrets");
		AobReplaceASM(ctx, $"mov dword ptr [esi+{offA}],1\nmov dword ptr [esi+{offB}],1", $"mov dword ptr [esi+{offA}],9999\nmov dword ptr [esi+{offB}],9999");
		IsEnabled = true;
	}
	public override void Disable(GameContext ctx)
	{
		int offA = GetOffset(ctx, "Terraria.Player", "maxMinions");
		int offB = GetOffset(ctx, "Terraria.Player", "maxTurrets");
		AobReplaceASM(ctx, $"mov dword ptr [esi+{offA}],9999\nmov dword ptr [esi+{offB}],9999", $"mov dword ptr [esi+{offA}],1\nmov dword ptr [esi+{offB}],1");
		IsEnabled = false;
	}
}

public class InfiniteMaking : BaseFunction
{
    // 特征码分析：
    // FF 4B 60       -> dec [ebx+60]        (目标：减1)
    // 83 7B 60 00    -> cmp [ebx+60], 0     (检查)
    // 7F 1F          -> jg 1F               (跳转)
    // 这个组合在当前版本是唯一的，非常完美。
    private const string Sig = "29 43 60 8B 45 10 33 D2";

    public override bool CanDisable => true;
    public override void ApplyLocalization(string culture)
    {
        Name = culture switch
        {
            "zh" => "制作不消耗",
            _ => "制作不消耗",
        };
    }

    public override void Enable(GameContext ctx)
    {
        AobReplace(ctx, Sig, "90 90 90");

        IsEnabled = true;
    }

    public override void Disable(GameContext ctx)
    {

        string patchedSig = "90 90 90 8B 45 10 33 D2";
        AobReplace(ctx, patchedSig, "29 43 60");

        IsEnabled = false;
    }
}
public class InfiniteItems : BaseFunction
{
    // 特征码分析：
    // FF 4B 60       -> dec [ebx+60]        (目标：减1)
    // 83 7B 60 00    -> cmp [ebx+60], 0     (检查)
    // 7F 1F          -> jg 1F               (跳转)
    // 这个组合在当前版本是唯一的，非常完美。
    private const string Sig = "FF 4B 60 83 7B 60 00 7F 1F";

    public override bool CanDisable => true;
    public override void ApplyLocalization(string culture)
    {
        Name = culture switch
        {
            "zh" => "无限物品",
            _ => "无限物品",
        };
        // 可以加个 Tooltip 说明这不仅是弹药，所有消耗品都有效
    }

    public override void Enable(GameContext ctx)
    {
        // 直接搜索你的完美特征码，把开头的 3 个字节 (FF 4B 60) 替换为 NOP (90 90 90)
        // AobReplace 只需要提供替换的字节，后续匹配上的字节会自动保留
        AobReplace(ctx, Sig, "90 90 90");

        IsEnabled = true;
    }

    public override void Disable(GameContext ctx)
    {
        // 还原逻辑：
        // 搜索被改过的特征码 (开头变成了 90 90 90)，然后把头改回 FF 4B 60
        string patchedSig = "90 90 90 83 7B 60 00 7F 1F";
        AobReplace(ctx, patchedSig, "FF 4B 60");

        IsEnabled = false;
    }
}
public class InfiniteAmmo : BaseFunction
{
    // 特征码分析：
    // FF 4B 60       -> dec [ebx+60]        (目标：减1)
    // 83 7B 60 00    -> cmp [ebx+60], 0     (检查)
    // 7F 1F          -> jg 1F               (跳转)
    // 这个组合在当前版本是唯一的，非常完美。
    private const string Sig = "FF 48 60 8B 45 DC 83 78 60 00";

    public override bool CanDisable => true;
    public override void ApplyLocalization(string culture)
    {
        Name = culture switch
        {
            "zh" => "无限弹药",
            _ => "无限弹药",
        };
        // 可以加个 Tooltip 说明这不仅是弹药，所有消耗品都有效
    }

    public override void Enable(GameContext ctx)
    {
        // 直接搜索你的完美特征码，把开头的 3 个字节 (FF 4B 60) 替换为 NOP (90 90 90)
        // AobReplace 只需要提供替换的字节，后续匹配上的字节会自动保留
        AobReplace(ctx, Sig, "90 90 90");

        IsEnabled = true;
    }

    public override void Disable(GameContext ctx)
    {
        // 还原逻辑：
        // 搜索被改过的特征码 (开头变成了 90 90 90)，然后把头改回 FF 4B 60
        string patchedSig = "90 90 90 8B 45 DC 83 78 60 00";
        AobReplace(ctx, patchedSig, "FF 48 60");

        IsEnabled = false;
    }
}
public class OneKeyGainBuff : BaseFunction
{
    // 特征码分析：
    // FF 4B 60       -> dec [ebx+60]        (目标：减1)
    // 83 7B 60 00    -> cmp [ebx+60], 0     (检查)
    // 7F 1F          -> jg 1F               (跳转)
    // 这个组合在当前版本是唯一的，非常完美。
    private const string Sig = "FF 4E 60 83 7E 60 00 7F 09";

    public override bool CanDisable => true;
    public override void ApplyLocalization(string culture)
    {
        Name = culture switch
        {
            "zh" => "B键增益不减",
            _ => "B键增益不减",
        };
    }

    public override void Enable(GameContext ctx)
    {
        AobReplace(ctx, Sig, "90 90 90");

        IsEnabled = true;
    }

    public override void Disable(GameContext ctx)
    {

        string patchedSig = "90 90 90 83 7E 60 00 7F 09";
        AobReplace(ctx, patchedSig, "FF 4E 60");

        IsEnabled = false;
    }
}
public class InfiniteAll : BaseFunction
{
    // 引用那4个子功能的实例
    private InfiniteItems _infiniteItems;
    private InfiniteAmmo _infiniteAmmo;
    private InfiniteMaking _infiniteMaking;
    private OneKeyGainBuff _oneKeyGainBuff;

    public override bool CanDisable => true;
    public override void ApplyLocalization(string culture)
    {
        switch (culture)
        {
            case "zh":
                Name = "无限全家桶";
                Tooltip = "一键开启：无限物品 + 无限弹药 + 制作不消耗 + B键增益不消耗";
                break;
            case "en":
            default:
                Name = "无限全家桶";
                Tooltip = "一键开启：无限物品 + 无限弹药 + 制作不消耗 + B键增益不消耗";
                break;
        }
    }
    public override void Enable(GameContext ctx)
    {
        // 我们利用 C# 的反射或者直接实例化来运行它们的逻辑
        // 为了代码复用，最简单的方法是直接创建它们的实例并调用 Enable

        // 1. 实例化子功能
        _infiniteItems = new InfiniteItems();
        _infiniteAmmo = new InfiniteAmmo();
        _infiniteMaking = new InfiniteMaking();
        _oneKeyGainBuff = new OneKeyGainBuff();

        // 2. 依次开启 (Try-Catch 防止某个特征码失效导致全部挂掉)
        TryEnable(_infiniteItems, ctx);
        TryEnable(_infiniteAmmo, ctx);
        TryEnable(_infiniteMaking, ctx);
        TryEnable(_oneKeyGainBuff, ctx);

        IsEnabled = true;
    }
    public override void Disable(GameContext ctx)
    {
        // 依次关闭
        TryDisable(_infiniteItems, ctx);
        TryDisable(_infiniteAmmo, ctx);
        TryDisable(_infiniteMaking, ctx);
        TryDisable(_oneKeyGainBuff, ctx);

        // 清空引用
        _infiniteItems = null;
        _infiniteAmmo = null;
        _infiniteMaking = null;
        _oneKeyGainBuff = null;

        IsEnabled = false;
    }
    // 辅助方法：安全开启
    private void TryEnable(BaseFunction func, GameContext ctx)
    {
        try
        {
            // 如果你希望这个“总开关”开启时，界面上那4个独立的开关也同步变绿，
            // 你需要去遍历 ctx.Functions 列表找到它们并修改状态。
            // 但这里我们简单处理，直接运行逻辑。
            func.Enable(ctx);
        }
        catch { /* 忽略错误，保证其他功能能开 */ }
    }
    // 辅助方法：安全关闭
    private void TryDisable(BaseFunction func, GameContext ctx)
    {
        if (func == null) return;
        try
        {
            if (func.IsEnabled) func.Disable(ctx);
        }
        catch { }
    }
}
public class InfiniteFlyTime : BaseFunction
{
	public override bool CanDisable => true;
	public override void ApplyLocalization(string culture)
	{
		Name = culture switch
		{
			"zh" => "无限飞行时间",
			_ => "无限飞行时间",
		};
	}
    private byte[] _originalBytes;
    private nuint _targetAddr = 0;
    public override void Enable(GameContext ctx)
	{
		int off1 = GetOffset(ctx, "Terraria.Player", "wingTime");
		int off2 = GetOffset(ctx, "Terraria.Player", "empressBrooch");
        string wingOffHex = AobscanHelper.GetMByteCode(off1);
        string broochOffHex = AobscanHelper.GetMByteCode(off2);
        //MessageBox.Show($"Debug Info:\n" +
        //            $"wingTime Offset: {off1} (Hex: 0x{off1:X})\n" +
        //            $"empressBrooch Offset: {off2} (Hex: 0x{off2:X})");
        //AobReplace(ctx, $"D9 9E {wingOffHex} 80 BE {broochOffHex} 00", "90 90 90 90 90 90");
        string sig = $"D9 ** {wingOffHex} 80 ** {broochOffHex} 00";
        var matches = Aobscan(ctx, sig).ToList();

		if (matches.Count > 0)
		{
			_targetAddr = matches[0];

			// 5. 备份原始的 6 个字节 (D9 9E E0 02 00 00)
			// 这样 Disable 的时候能完美还原，不用担心猜错寄存器
			_originalBytes = ScriptHelper.ReadBytes(ctx, _targetAddr, 6).ToArray();
			string debugHex = BitConverter.ToString(_originalBytes);
			//MessageBox.Show($"找到地址(sig?): 0x{_targetAddr:X}\n" +
			//				$"找到的6 个字节sig: {debugHex}\n\n" +
			//				$"期待的: D9-9E-{off1:X2}-..-..-00 (请确认!)");
			// 6. 写入修改 (Patch)
			// 使用 DD D8 (fstp st0) 加上 4个 NOP 填充，总共 6 字节
			// 这样既不写入内存，又清空了浮点栈，防止游戏崩溃
			ScriptHelper.WriteBytes(ctx, _targetAddr, new byte[] { 0xDD, 0xD8, 0x90, 0x90, 0x90, 0x90 });

			IsEnabled = true;
		}
		else 
		{
            MessageBox.Show("无限飞行开启失败！未找到地址！");
            IsEnabled = false;
        }
	}
    public override void Disable(GameContext ctx)
    {
        if (_targetAddr != 0 && _originalBytes != null)
        {
            // 还原备份的原始字节
            ScriptHelper.WriteBytes(ctx, _targetAddr, _originalBytes);

            _targetAddr = 0;
            _originalBytes = null;
        }
        IsEnabled = false;
    }
}

public class ImmuneToDebuffs : BaseFunction
{
	public override bool CanDisable => true;
	public override void ApplyLocalization(string culture)
	{
		Name = culture switch
		{
			"zh" => "免疫Debuff",
			_ => "Immune to debuffs",
		};
	}
	public override void Enable(GameContext ctx)
	{
		nuint a = GetFunctionAddress(ctx, "Terraria.Player", "AddBuff");
		if (Read<byte>(ctx, a) == 0xE9)
			return;
		InlineHook.Hook(ctx.HContext,
			AssemblySnippet.FromCode(
				new AssemblyCode[]{
					(Instruction)$"pushad",
					(Instruction)$"mov ebx,{ctx.Debuff.BaseAddress}",
					(Instruction)$"cmp byte ptr [ebx+edx+8],0",
					(Instruction)$"je end",
					(Instruction)$"popad",
					(Instruction)$"ret 8",
					(Instruction)$"end:",
					(Instruction)$"popad",
				}), new HookParameters(a, 0x1000));
		IsEnabled = true;
	}
	public override void Disable(GameContext ctx)
	{
		nuint a = GetFunctionAddress(ctx, "Terraria.Player", "AddBuff");
		InlineHook.FreeHook(ctx.HContext, a);
		IsEnabled = false;
	}
}

public class HighLight : BaseFunction
{
	public override bool CanDisable => true;
	public override void ApplyLocalization(string culture)
	{
		switch (culture)
		{
			case "zh":
				Name = "全屏高亮";
				Tooltip = "请将游戏内视频设置为\"彩色\"";
				break;
			case "en":
			default:
				Name = "High Light";
				Tooltip = "Please set Video -> Lighting to \"Color\"";
				break;
		}
	}
	public override void Enable(GameContext ctx)
	{
		nuint[] a = Aobscan(
			ctx,
			@"C7 ** ** ******** D9 07 D9 45 F0 DF F1 DD D8 7A").ToArray();
		if (!a.Any())
			return;
		InlineHook.Hook(ctx.HContext,
			AssemblySnippet.FromASMCode(
				@"mov dword ptr[ebp-0x10],0x3F800000
mov dword ptr[ebp-0x14],0x3F800000
mov dword ptr[ebp-0x18],0x3F800000"
),
				new HookParameters(a[0] + 7, 0x1000));
		IsEnabled = true;
	}
	public override void Disable(GameContext ctx)
	{
		nuint[] a = Aobscan(ctx, "C7 ** ** ******** E9 ** ** ** ** DF F1 DD D8 7A").ToArray();
		if (!a.Any())
			return;
		InlineHook.FreeHook(ctx.HContext, a[0] + 7);
		IsEnabled = false;
	}
}

public class GhostMode : BaseFunction
{
	public override bool CanDisable => true;
	public override void ApplyLocalization(string culture)
	{
		Name = culture switch
		{
			"zh" => "幽灵模式",
			_ => "Ghost Mode",
		};
	}
	public override void Enable(GameContext ctx)
	{
		ctx.MyPlayer.Ghost = true;
		IsEnabled = true;
	}
	public override void Disable(GameContext ctx)
	{
		ctx.MyPlayer.Ghost = false;
		IsEnabled = false;
	}
}
public class BuiltIn_1 : FunctionCategory
{
	public override string Category => "Basic1";
	public BuiltIn_1()
	{
		this["zh"] = "基础1";
		this["en"] = "Basic 1";

		Add<CreativeMenu>();
		Add<UnlockAllDuplications>();
		Add<InfiniteLife>();
		Add<InfiniteMana>();
		Add<InfiniteOxygen>();
		Add<InfiniteMinion>();
        Add<InfiniteMaking>();
        Add<InfiniteItems>();
        Add<InfiniteAmmo>();
        Add<OneKeyGainBuff>();
        Add<InfiniteAll>();
		Add<InfiniteFlyTime>();
		Add<ImmuneToDebuffs>();
		Add<HighLight>();
		Add<GhostMode>();
	}
}
