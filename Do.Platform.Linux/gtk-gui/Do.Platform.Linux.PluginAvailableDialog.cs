// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace Do.Platform.Linux {
    
    
    public partial class PluginAvailableDialog {
        
        private Gtk.VBox vbox2;
        
        private Gtk.HBox hbox1;
        
        private Gtk.Image title_img;
        
        private Gtk.Alignment title_align;
        
        private Gtk.Label title_lbl;
        
        private Gtk.Alignment body_align;
        
        private Gtk.Label body_lbl;
        
        private Gtk.Alignment link_align;
        
        private Gtk.VBox link_vbox;
        
        private Gtk.Alignment ask_align;
        
        private Gtk.CheckButton ask_chk;
        
        private Gtk.Button buttonCancel;
        
        private Gtk.Button install_btn;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget Do.Platform.Linux.PluginAvailableDialog
            this.Name = "Do.Platform.Linux.PluginAvailableDialog";
            this.Icon = Stetic.IconLoader.LoadIcon(this, "distributor-logo", Gtk.IconSize.Menu, 16);
            this.WindowPosition = ((Gtk.WindowPosition)(4));
            this.BorderWidth = ((uint)(5));
            // Internal child Do.Platform.Linux.PluginAvailableDialog.VBox
            Gtk.VBox w1 = this.VBox;
            w1.Name = "dialog1_VBox";
            w1.BorderWidth = ((uint)(2));
            // Container child dialog1_VBox.Gtk.Box+BoxChild
            this.vbox2 = new Gtk.VBox();
            this.vbox2.Name = "vbox2";
            this.vbox2.Spacing = 6;
            // Container child vbox2.Gtk.Box+BoxChild
            this.hbox1 = new Gtk.HBox();
            this.hbox1.Name = "hbox1";
            this.hbox1.Spacing = 6;
            // Container child hbox1.Gtk.Box+BoxChild
            this.title_img = new Gtk.Image();
            this.title_img.Name = "title_img";
            this.title_img.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gnome-do", Gtk.IconSize.Dialog, 48);
            this.hbox1.Add(this.title_img);
            Gtk.Box.BoxChild w2 = ((Gtk.Box.BoxChild)(this.hbox1[this.title_img]));
            w2.Position = 0;
            w2.Expand = false;
            w2.Fill = false;
            // Container child hbox1.Gtk.Box+BoxChild
            this.title_align = new Gtk.Alignment(0.5F, 0.5F, 1F, 1F);
            this.title_align.Name = "title_align";
            this.title_align.LeftPadding = ((uint)(7));
            // Container child title_align.Gtk.Container+ContainerChild
            this.title_lbl = new Gtk.Label();
            this.title_lbl.Name = "title_lbl";
            this.title_lbl.LabelProp = Mono.Unix.Catalog.GetString("<b><span size=\"x-large\">There's a Do plugin for that!</span></b>");
            this.title_lbl.UseMarkup = true;
            this.title_align.Add(this.title_lbl);
            this.hbox1.Add(this.title_align);
            Gtk.Box.BoxChild w4 = ((Gtk.Box.BoxChild)(this.hbox1[this.title_align]));
            w4.Position = 1;
            w4.Expand = false;
            w4.Fill = false;
            this.vbox2.Add(this.hbox1);
            Gtk.Box.BoxChild w5 = ((Gtk.Box.BoxChild)(this.vbox2[this.hbox1]));
            w5.Position = 0;
            w5.Expand = false;
            w5.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.body_align = new Gtk.Alignment(0.5F, 0.5F, 1F, 1F);
            this.body_align.Name = "body_align";
            this.body_align.LeftPadding = ((uint)(62));
            // Container child body_align.Gtk.Container+ContainerChild
            this.body_lbl = new Gtk.Label();
            this.body_lbl.Name = "body_lbl";
            this.body_lbl.Xalign = 0F;
            this.body_lbl.LabelProp = Mono.Unix.Catalog.GetString("A Do plugin for {0} is available for installation. Would you like us to enable it for you?");
            this.body_lbl.Wrap = true;
            this.body_align.Add(this.body_lbl);
            this.vbox2.Add(this.body_align);
            Gtk.Box.BoxChild w7 = ((Gtk.Box.BoxChild)(this.vbox2[this.body_align]));
            w7.Position = 1;
            w7.Expand = false;
            w7.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.link_align = new Gtk.Alignment(0.5F, 0.5F, 1F, 1F);
            this.link_align.Name = "link_align";
            this.link_align.LeftPadding = ((uint)(55));
            // Container child link_align.Gtk.Container+ContainerChild
            this.link_vbox = new Gtk.VBox();
            this.link_vbox.Name = "link_vbox";
            this.link_align.Add(this.link_vbox);
            this.vbox2.Add(this.link_align);
            Gtk.Box.BoxChild w9 = ((Gtk.Box.BoxChild)(this.vbox2[this.link_align]));
            w9.Position = 2;
            // Container child vbox2.Gtk.Box+BoxChild
            this.ask_align = new Gtk.Alignment(0.5F, 0.5F, 1F, 1F);
            this.ask_align.Name = "ask_align";
            this.ask_align.LeftPadding = ((uint)(57));
            // Container child ask_align.Gtk.Container+ContainerChild
            this.ask_chk = new Gtk.CheckButton();
            this.ask_chk.CanFocus = true;
            this.ask_chk.Name = "ask_chk";
            this.ask_chk.Label = Mono.Unix.Catalog.GetString("Don't ask me about Do plugins again.");
            this.ask_chk.DrawIndicator = true;
            this.ask_chk.UseUnderline = true;
            this.ask_align.Add(this.ask_chk);
            this.vbox2.Add(this.ask_align);
            Gtk.Box.BoxChild w11 = ((Gtk.Box.BoxChild)(this.vbox2[this.ask_align]));
            w11.Position = 3;
            w11.Expand = false;
            w11.Fill = false;
            w1.Add(this.vbox2);
            Gtk.Box.BoxChild w12 = ((Gtk.Box.BoxChild)(w1[this.vbox2]));
            w12.Position = 0;
            // Internal child Do.Platform.Linux.PluginAvailableDialog.ActionArea
            Gtk.HButtonBox w13 = this.ActionArea;
            w13.Name = "dialog1_ActionArea";
            w13.Spacing = 6;
            w13.BorderWidth = ((uint)(5));
            w13.LayoutStyle = ((Gtk.ButtonBoxStyle)(4));
            // Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
            this.buttonCancel = new Gtk.Button();
            this.buttonCancel.CanDefault = true;
            this.buttonCancel.CanFocus = true;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseStock = true;
            this.buttonCancel.UseUnderline = true;
            this.buttonCancel.Label = "gtk-cancel";
            this.AddActionWidget(this.buttonCancel, -6);
            Gtk.ButtonBox.ButtonBoxChild w14 = ((Gtk.ButtonBox.ButtonBoxChild)(w13[this.buttonCancel]));
            w14.Expand = false;
            w14.Fill = false;
            // Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
            this.install_btn = new Gtk.Button();
            this.install_btn.CanDefault = true;
            this.install_btn.CanFocus = true;
            this.install_btn.Name = "install_btn";
            this.install_btn.UseUnderline = true;
            // Container child install_btn.Gtk.Container+ContainerChild
            Gtk.Alignment w15 = new Gtk.Alignment(0.5F, 0.5F, 0F, 0F);
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            Gtk.HBox w16 = new Gtk.HBox();
            w16.Spacing = 2;
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Image w17 = new Gtk.Image();
            w17.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gtk-ok", Gtk.IconSize.Button, 16);
            w16.Add(w17);
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Label w19 = new Gtk.Label();
            w19.LabelProp = Mono.Unix.Catalog.GetString("_Install");
            w19.UseUnderline = true;
            w16.Add(w19);
            w15.Add(w16);
            this.install_btn.Add(w15);
            this.AddActionWidget(this.install_btn, -5);
            Gtk.ButtonBox.ButtonBoxChild w23 = ((Gtk.ButtonBox.ButtonBoxChild)(w13[this.install_btn]));
            w23.Position = 1;
            w23.Expand = false;
            w23.Fill = false;
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 486;
            this.DefaultHeight = 218;
            this.Show();
            this.ask_chk.Toggled += new System.EventHandler(this.OnAskChkToggled);
            this.buttonCancel.Clicked += new System.EventHandler(this.OnButtonCancelClicked);
            this.install_btn.Clicked += new System.EventHandler(this.OnInstallBtnClicked);
        }
    }
}