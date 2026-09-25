using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Eclipse.Launcher
{
    // Palette and font follow the in-game title screen (Assets/Scripts/Eclipse/UI/TitleScreen.cs)
    // and mod UI (ModUiView.cs) so the launcher reads as part of the same game.
    internal static class Theme
    {
        public static readonly Color Ink = Color.FromArgb(30, 25, 22);
        public static readonly Color Paper = Color.FromArgb(223, 207, 177);
        public static readonly Color Red = Color.FromArgb(147, 39, 31);
        public static readonly Color RedHover = Color.FromArgb(171, 48, 37);
        public static readonly Color RedPressed = Color.FromArgb(105, 30, 24);
        public static readonly Color Gold = Color.FromArgb(213, 165, 62);
        public static readonly Color Track = Color.FromArgb(48, 31, 20);
        public static readonly Color Wood = Color.FromArgb(73, 43, 29);
        public static readonly Color Grain = Color.FromArgb(103, 66, 43);
        public static readonly Color Frame = Color.FromArgb(164, 120, 66);
        public static readonly Color Panel = Color.FromArgb(39, 32, 27);
        public static readonly Color Muted = Color.FromArgb(150, 134, 110);
        public static readonly Color Disabled = Color.FromArgb(96, 85, 71);
        public static readonly Font Body = new Font("Segoe UI", 9.5f), BodyUnderline = new Font("Segoe UI", 9.5f, FontStyle.Underline);

        // Layout is written in 96 DPI pixels and multiplied by this. Program.Main marks the
        // process DPI aware before Theme is first used, so DpiX is the real system DPI.
        public static readonly float Scale = ReadScale();
        private static readonly PrivateFontCollection fonts = LoadFonts();
        public static readonly Font TitleFont = Display(20), PlayFont = Display(17), ButtonFont = Display(14), SmallFont = Display(12);

        private static float ReadScale()
        {
            using (var g = Graphics.FromHwnd(IntPtr.Zero)) return g.DpiX / 96f;
        }
        public static int S(float pixels) { return (int)Math.Round(pixels * Scale); }
        public static Rectangle S(int x, int y, int width, int height) { return new Rectangle(S(x), S(y), S(width), S(height)); }

        // AGOpusBold and icon.png are embedded by BuildLauncher.ps1.
        private static PrivateFontCollection LoadFonts()
        {
            var collection = new PrivateFontCollection();
            using (var stream = typeof(Theme).Assembly.GetManifestResourceStream("AGOpusBold.ttf"))
            {
                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                // GDI+ reads from this memory for the life of the process, so it is never freed.
                IntPtr memory = Marshal.AllocCoTaskMem(data.Length);
                Marshal.Copy(data, 0, memory, data.Length);
                collection.AddMemoryFont(memory, data.Length);
            }
            return collection;
        }

        private static Font Display(float pixels)
        {
            var family = fonts.Families[0];
            return new Font(family, pixels * Scale, family.IsStyleAvailable(FontStyle.Bold) ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Pixel);
        }

        // Memory fonts only render through GDI+, so display text uses DrawString rather than TextRenderer.
        public static void DrawDisplay(Graphics g, string text, Font font, Rectangle bounds, Color color, StringAlignment align)
        {
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            using (var brush = new SolidBrush(color))
            using (var format = new StringFormat(StringFormatFlags.NoWrap) { Alignment = align, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter })
                g.DrawString(text, font, brush, bounds, format);
        }

        private static Bitmap LoadIcon()
        {
            using (var stream = typeof(Theme).Assembly.GetManifestResourceStream("icon.png"))
            using (var image = Image.FromStream(stream))
                return new Bitmap(image);
        }

        // The icon is a white silhouette on black; map its brightness to alpha and tint it paper.
        public static Image Emblem(int size)
        {
            using (var source = LoadIcon())
            {
                var result = new Bitmap(size, size, PixelFormat.Format32bppArgb);
                var matrix = new ColorMatrix(new[]
                {
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 0 },
                    new float[] { 0, 0, 0, 0, 0 },
                    new float[] { 0, 0, 0, 0, 0 },
                    new float[] { Paper.R / 255f, Paper.G / 255f, Paper.B / 255f, 0, 1 },
                });
                using (var g = Graphics.FromImage(result))
                using (var attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(matrix);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    // Crop the padding around the figure.
                    int inset = source.Width * 13 / 100;
                    g.DrawImage(source, new Rectangle(0, 0, size, size), inset, inset, source.Width - inset * 2, source.Height - inset * 2, GraphicsUnit.Pixel, attributes);
                }
                return result;
            }
        }

        public static Icon WindowIcon()
        {
            using (var source = LoadIcon())
            using (var small = new Bitmap(source, 32, 32))
                return Icon.FromHandle(small.GetHicon());
        }
    }

    internal class ThemedButton : Button
    {
        protected bool Hover, Pressed;
        private bool accent;
        public bool Primary;
        // Gold outline and text, used to point at an available update.
        public bool Accent { get { return accent; } set { if (accent != value) { accent = value; Invalidate(); } } }

        public ThemedButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            FlatStyle = FlatStyle.Flat;
            Cursor = Cursors.Hand;
        }
        protected override void OnMouseEnter(EventArgs e) { Hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { Hover = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { if (e.Button == MouseButtons.Left) { Pressed = true; Invalidate(); } base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { Pressed = false; Invalidate(); base.OnMouseUp(e); }
        protected override void OnEnabledChanged(EventArgs e) { Hover = Pressed = false; Invalidate(); base.OnEnabledChanged(e); }
        protected override void OnTextChanged(EventArgs e) { Invalidate(); base.OnTextChanged(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            Color fill, border, text = Theme.Paper;
            if (!Enabled) { fill = Theme.Ink; border = Theme.Wood; text = Theme.Disabled; }
            else if (Pressed) fill = border = Theme.RedPressed;
            else if (Hover) fill = border = Primary ? Theme.RedHover : Theme.Red;
            else if (Primary) { fill = Theme.Red; border = Theme.RedHover; }
            else if (Accent) { fill = Theme.Panel; border = text = Theme.Gold; }
            else { fill = Theme.Panel; border = Theme.Grain; }
            using (var brush = new SolidBrush(fill)) g.FillRectangle(brush, ClientRectangle);
            using (var pen = new Pen(border)) g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            if (Focused && ShowFocusCues)
                using (var pen = new Pen(Theme.Gold)) g.DrawRectangle(pen, 2, 2, Width - 5, Height - 5);
            Theme.DrawDisplay(g, Text, Primary ? Theme.PlayFont : Theme.ButtonFont, new Rectangle(6, 1, Width - 12, Height), text, StringAlignment.Center);
        }
    }

    internal sealed class CaptionButton : ThemedButton
    {
        private readonly bool close;
        public CaptionButton(bool closeGlyph) { close = closeGlyph; TabStop = false; Cursor = Cursors.Default; }
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            Color fill = Pressed ? Theme.RedPressed : Hover ? (close ? Theme.Red : Theme.Grain) : Theme.Wood;
            using (var brush = new SolidBrush(fill)) g.FillRectangle(brush, ClientRectangle);
            int cx = Width / 2, cy = Height / 2, r = Theme.S(5);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var pen = new Pen(Theme.Paper, 1.4f * Theme.Scale))
            {
                if (close) { g.DrawLine(pen, cx - r, cy - r, cx + r, cy + r); g.DrawLine(pen, cx + r, cy - r, cx - r, cy + r); }
                else g.DrawLine(pen, cx - r, cy, cx + r, cy);
            }
        }
    }

    // Secondary action drawn as a text link so it doesn't compete with Play and Update.
    internal sealed class LinkButton : ThemedButton
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            using (var brush = new SolidBrush(Theme.Ink)) g.FillRectangle(brush, ClientRectangle);
            bool active = Enabled && (Hover || Focused && ShowFocusCues);
            // Unavailable links stay readable in grey so players can still find Roll back.
            TextRenderer.DrawText(g, Text, active ? Theme.BodyUnderline : Theme.Body, ClientRectangle, !Enabled ? Theme.Muted : active ? Theme.Paper : Theme.Gold,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }
    }

    internal sealed class ThemedCheckBox : CheckBox
    {
        private bool hover;
        public ThemedCheckBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Cursor = Cursors.Hand;
        }
        protected override void OnMouseEnter(EventArgs e) { hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { hover = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            using (var brush = new SolidBrush(Theme.Ink)) g.FillRectangle(brush, ClientRectangle);
            int size = Theme.S(14), inset = Theme.S(3), gap = Theme.S(8);
            var box = new Rectangle(0, (Height - size) / 2, size, size);
            using (var brush = new SolidBrush(Theme.Track)) g.FillRectangle(brush, box);
            Color border = !Enabled ? Theme.Wood : Focused && ShowFocusCues ? Theme.Gold : hover ? Theme.Frame : Theme.Grain;
            using (var pen = new Pen(border)) g.DrawRectangle(pen, box.X, box.Y, size - 1, size - 1);
            if (Checked)
                using (var brush = new SolidBrush(Enabled ? Theme.Gold : Theme.Disabled)) g.FillRectangle(brush, box.X + inset, box.Y + inset, size - inset * 2, size - inset * 2);
            // A setting rather than an action, so the label stays muted until hovered.
            TextRenderer.DrawText(g, Text, Theme.Body, new Rectangle(size + gap, 0, Width - size - gap, Height), !Enabled ? Theme.Disabled : hover ? Theme.Paper : Theme.Muted,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
    }

    // Two-segment stable/beta picker used in place of a ComboBox.
    internal sealed class ChannelSwitch : Control
    {
        private readonly string[] items;
        private int selected = -1, hover = -1;
        public event EventHandler SelectedIndexChanged;

        public ChannelSwitch(params string[] choices)
        {
            items = choices;
            TabStop = true;
            Cursor = Cursors.Hand;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
        }
        public string SelectedItem
        {
            get { return selected < 0 ? null : items[selected]; }
            set { Select(Array.IndexOf(items, value)); }
        }
        private void Select(int index)
        {
            if (index == selected) return;
            selected = index;
            Invalidate();
            if (SelectedIndexChanged != null) SelectedIndexChanged(this, EventArgs.Empty);
        }
        private int IndexAt(int x) { return Math.Max(0, Math.Min(items.Length - 1, x * items.Length / Width)); }
        protected override void OnMouseMove(MouseEventArgs e) { int i = IndexAt(e.X); if (i != hover) { hover = i; Invalidate(); } base.OnMouseMove(e); }
        protected override void OnMouseLeave(EventArgs e) { hover = -1; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { if (e.Button == MouseButtons.Left) { Focus(); Select(IndexAt(e.X)); } base.OnMouseDown(e); }
        protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }
        protected override void OnGotFocus(EventArgs e) { Invalidate(); base.OnGotFocus(e); }
        protected override void OnLostFocus(EventArgs e) { Invalidate(); base.OnLostFocus(e); }
        protected override bool IsInputKey(Keys key) { return key == Keys.Left || key == Keys.Right || base.IsInputKey(key); }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                Select(Math.Max(0, Math.Min(items.Length - 1, selected + (e.KeyCode == Keys.Right ? 1 : -1))));
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            using (var brush = new SolidBrush(Theme.Panel)) g.FillRectangle(brush, ClientRectangle);
            for (int i = 0; i < items.Length; i++)
            {
                int left = i * Width / items.Length, right = (i + 1) * Width / items.Length;
                var segment = new Rectangle(left, 0, right - left, Height);
                Color text = !Enabled ? Theme.Disabled : Theme.Muted;
                if (i == selected)
                {
                    using (var brush = new SolidBrush(Enabled ? Theme.Red : Theme.Wood)) g.FillRectangle(brush, segment);
                    text = Enabled ? Theme.Paper : Theme.Muted;
                }
                else if (i == hover && Enabled) text = Theme.Paper;
                Theme.DrawDisplay(g, items[i].ToUpperInvariant(), Theme.SmallFont, new Rectangle(segment.X, segment.Y + 1, segment.Width, segment.Height), text, StringAlignment.Center);
            }
            Color border = Focused && ShowFocusCues ? Theme.Gold : Enabled ? Theme.Grain : Theme.Wood;
            using (var pen = new Pen(border)) g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        }
    }

    internal sealed class ThemedProgress : Control
    {
        private int value;
        public ThemedProgress()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Visible = false;
        }
        // Only shown while there is progress to report.
        public int Value
        {
            get { return value; }
            set { int next = Math.Max(0, Math.Min(100, value)); if (next != this.value) { this.value = next; Visible = next > 0; Invalidate(); } }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            using (var brush = new SolidBrush(Theme.Track)) g.FillRectangle(brush, ClientRectangle);
            using (var brush = new SolidBrush(Theme.Gold)) g.FillRectangle(brush, 0, 0, Width * value / 100, Height);
        }
    }

    internal sealed class Card : Panel
    {
        public Card() { BackColor = Theme.Panel; DoubleBuffered = true; ResizeRedraw = true; }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (var pen = new Pen(Theme.Wood)) e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            using (var brush = new SolidBrush(Theme.Frame)) e.Graphics.FillRectangle(brush, 0, 0, Theme.S(3), Height);
        }
    }
}
