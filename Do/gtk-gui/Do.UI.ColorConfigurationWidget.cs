// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace Do.UI {
    
    
    public partial class ColorConfigurationWidget {
        
        private Gtk.VBox vbox2;
        
        private Gtk.Frame frame1;
        
        private Gtk.Alignment GtkAlignment;
        
        private Gtk.Alignment alignment2;
        
        private Gtk.VBox vbox1;
        
        private Gtk.Alignment alignment1;
        
        private Gtk.Table table1;
        
        private Gtk.HBox hbox3;
        
        private Gtk.ColorButton background_colorbutton;
        
        private Gtk.Button clear_background;
        
        private Gtk.Label label8;
        
        private Gtk.ComboBox theme_combo;
        
        private Gtk.Label theme_lbl;
        
        private Gtk.VBox vbox3;
        
        private Gtk.VBox vbox4;
        
        private Gtk.HBox hbox2;
        
        private Gtk.CheckButton pin_check;
        
        private Gtk.CheckButton shadow_check;
        
        private Gtk.CheckButton animation_check;
        
        private Gtk.Alignment theme_configuration_container;
        
        private Gtk.VBox composite_warning_widget;
        
        private Gtk.Label label1;
        
        private Gtk.HButtonBox hbuttonbox1;
        
        private Gtk.Button composite_warning_info_btn;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget Do.UI.ColorConfigurationWidget
            Stetic.BinContainer.Attach(this);
            this.Name = "Do.UI.ColorConfigurationWidget";
            // Container child Do.UI.ColorConfigurationWidget.Gtk.Container+ContainerChild
            this.vbox2 = new Gtk.VBox();
            this.vbox2.Name = "vbox2";
            this.vbox2.Spacing = 6;
            // Container child vbox2.Gtk.Box+BoxChild
            this.frame1 = new Gtk.Frame();
            this.frame1.Name = "frame1";
            this.frame1.ShadowType = ((Gtk.ShadowType)(0));
            // Container child frame1.Gtk.Container+ContainerChild
            this.GtkAlignment = new Gtk.Alignment(0F, 0F, 1F, 1F);
            this.GtkAlignment.Name = "GtkAlignment";
            this.GtkAlignment.LeftPadding = ((uint)(5));
            this.GtkAlignment.TopPadding = ((uint)(5));
            this.GtkAlignment.RightPadding = ((uint)(5));
            this.GtkAlignment.BottomPadding = ((uint)(10));
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            this.alignment2 = new Gtk.Alignment(0.5F, 0.5F, 1F, 1F);
            this.alignment2.Name = "alignment2";
            // Container child alignment2.Gtk.Container+ContainerChild
            this.vbox1 = new Gtk.VBox();
            this.vbox1.Name = "vbox1";
            this.vbox1.Spacing = 6;
            // Container child vbox1.Gtk.Box+BoxChild
            this.alignment1 = new Gtk.Alignment(0.5F, 0.5F, 1F, 1F);
            this.alignment1.Name = "alignment1";
            // Container child alignment1.Gtk.Container+ContainerChild
            this.table1 = new Gtk.Table(((uint)(2)), ((uint)(2)), false);
            this.table1.Name = "table1";
            this.table1.RowSpacing = ((uint)(6));
            this.table1.ColumnSpacing = ((uint)(16));
            // Container child table1.Gtk.Table+TableChild
            this.hbox3 = new Gtk.HBox();
            this.hbox3.Name = "hbox3";
            this.hbox3.Spacing = 6;
            // Container child hbox3.Gtk.Box+BoxChild
            this.background_colorbutton = new Gtk.ColorButton();
            this.background_colorbutton.CanFocus = true;
            this.background_colorbutton.Events = ((Gdk.EventMask)(784));
            this.background_colorbutton.Name = "background_colorbutton";
            this.background_colorbutton.UseAlpha = true;
            this.background_colorbutton.Alpha = 65535;
            this.hbox3.Add(this.background_colorbutton);
            Gtk.Box.BoxChild w1 = ((Gtk.Box.BoxChild)(this.hbox3[this.background_colorbutton]));
            w1.Position = 0;
            // Container child hbox3.Gtk.Box+BoxChild
            this.clear_background = new Gtk.Button();
            this.clear_background.CanFocus = true;
            this.clear_background.Name = "clear_background";
            this.clear_background.UseUnderline = true;
            this.clear_background.Relief = ((Gtk.ReliefStyle)(2));
            // Container child clear_background.Gtk.Container+ContainerChild
            Gtk.Alignment w2 = new Gtk.Alignment(0.5F, 0.5F, 0F, 0F);
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            Gtk.HBox w3 = new Gtk.HBox();
            w3.Spacing = 2;
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Image w4 = new Gtk.Image();
            w4.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gtk-clear", Gtk.IconSize.Menu, 16);
            w3.Add(w4);
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Label w6 = new Gtk.Label();
            w6.LabelProp = Mono.Unix.Catalog.GetString("_Reset");
            w6.UseUnderline = true;
            w3.Add(w6);
            w2.Add(w3);
            this.clear_background.Add(w2);
            this.hbox3.Add(this.clear_background);
            Gtk.Box.BoxChild w10 = ((Gtk.Box.BoxChild)(this.hbox3[this.clear_background]));
            w10.Position = 1;
            w10.Expand = false;
            w10.Fill = false;
            this.table1.Add(this.hbox3);
            Gtk.Table.TableChild w11 = ((Gtk.Table.TableChild)(this.table1[this.hbox3]));
            w11.TopAttach = ((uint)(1));
            w11.BottomAttach = ((uint)(2));
            w11.LeftAttach = ((uint)(1));
            w11.RightAttach = ((uint)(2));
            w11.XOptions = ((Gtk.AttachOptions)(4));
            w11.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label8 = new Gtk.Label();
            this.label8.Name = "label8";
            this.label8.Xalign = 0F;
            this.label8.LabelProp = Mono.Unix.Catalog.GetString("Background Color:");
            this.table1.Add(this.label8);
            Gtk.Table.TableChild w12 = ((Gtk.Table.TableChild)(this.table1[this.label8]));
            w12.TopAttach = ((uint)(1));
            w12.BottomAttach = ((uint)(2));
            w12.XOptions = ((Gtk.AttachOptions)(4));
            w12.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.theme_combo = Gtk.ComboBox.NewText();
            this.theme_combo.Name = "theme_combo";
            this.table1.Add(this.theme_combo);
            Gtk.Table.TableChild w13 = ((Gtk.Table.TableChild)(this.table1[this.theme_combo]));
            w13.LeftAttach = ((uint)(1));
            w13.RightAttach = ((uint)(2));
            w13.XOptions = ((Gtk.AttachOptions)(4));
            w13.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.theme_lbl = new Gtk.Label();
            this.theme_lbl.Name = "theme_lbl";
            this.theme_lbl.Xalign = 0F;
            this.theme_lbl.LabelProp = Mono.Unix.Catalog.GetString("_Theme:");
            this.theme_lbl.UseUnderline = true;
            this.table1.Add(this.theme_lbl);
            Gtk.Table.TableChild w14 = ((Gtk.Table.TableChild)(this.table1[this.theme_lbl]));
            w14.XOptions = ((Gtk.AttachOptions)(4));
            w14.YOptions = ((Gtk.AttachOptions)(4));
            this.alignment1.Add(this.table1);
            this.vbox1.Add(this.alignment1);
            Gtk.Box.BoxChild w16 = ((Gtk.Box.BoxChild)(this.vbox1[this.alignment1]));
            w16.Position = 0;
            w16.Expand = false;
            w16.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.vbox3 = new Gtk.VBox();
            this.vbox3.Name = "vbox3";
            this.vbox3.Spacing = 6;
            // Container child vbox3.Gtk.Box+BoxChild
            this.vbox4 = new Gtk.VBox();
            this.vbox4.Name = "vbox4";
            this.vbox4.Spacing = 6;
            // Container child vbox4.Gtk.Box+BoxChild
            this.hbox2 = new Gtk.HBox();
            this.hbox2.Name = "hbox2";
            this.hbox2.Homogeneous = true;
            this.hbox2.Spacing = 6;
            // Container child hbox2.Gtk.Box+BoxChild
            this.pin_check = new Gtk.CheckButton();
            this.pin_check.CanFocus = true;
            this.pin_check.Name = "pin_check";
            this.pin_check.Label = Mono.Unix.Catalog.GetString("Always show results window");
            this.pin_check.Active = true;
            this.pin_check.DrawIndicator = true;
            this.pin_check.UseUnderline = true;
            this.hbox2.Add(this.pin_check);
            Gtk.Box.BoxChild w17 = ((Gtk.Box.BoxChild)(this.hbox2[this.pin_check]));
            w17.Position = 0;
            this.vbox4.Add(this.hbox2);
            Gtk.Box.BoxChild w18 = ((Gtk.Box.BoxChild)(this.vbox4[this.hbox2]));
            w18.Position = 0;
            w18.Expand = false;
            w18.Fill = false;
            // Container child vbox4.Gtk.Box+BoxChild
            this.shadow_check = new Gtk.CheckButton();
            this.shadow_check.CanFocus = true;
            this.shadow_check.Name = "shadow_check";
            this.shadow_check.Label = Mono.Unix.Catalog.GetString("Show window shadow");
            this.shadow_check.DrawIndicator = true;
            this.shadow_check.UseUnderline = true;
            this.vbox4.Add(this.shadow_check);
            Gtk.Box.BoxChild w19 = ((Gtk.Box.BoxChild)(this.vbox4[this.shadow_check]));
            w19.Position = 1;
            w19.Expand = false;
            w19.Fill = false;
            // Container child vbox4.Gtk.Box+BoxChild
            this.animation_check = new Gtk.CheckButton();
            this.animation_check.CanFocus = true;
            this.animation_check.Name = "animation_check";
            this.animation_check.Label = Mono.Unix.Catalog.GetString("Animate window");
            this.animation_check.Active = true;
            this.animation_check.DrawIndicator = true;
            this.animation_check.UseUnderline = true;
            this.vbox4.Add(this.animation_check);
            Gtk.Box.BoxChild w20 = ((Gtk.Box.BoxChild)(this.vbox4[this.animation_check]));
            w20.Position = 2;
            w20.Expand = false;
            w20.Fill = false;
            this.vbox3.Add(this.vbox4);
            Gtk.Box.BoxChild w21 = ((Gtk.Box.BoxChild)(this.vbox3[this.vbox4]));
            w21.Position = 0;
            w21.Expand = false;
            w21.Fill = false;
            this.vbox1.Add(this.vbox3);
            Gtk.Box.BoxChild w22 = ((Gtk.Box.BoxChild)(this.vbox1[this.vbox3]));
            w22.Position = 1;
            w22.Expand = false;
            w22.Fill = false;
            this.alignment2.Add(this.vbox1);
            this.GtkAlignment.Add(this.alignment2);
            this.frame1.Add(this.GtkAlignment);
            this.vbox2.Add(this.frame1);
            Gtk.Box.BoxChild w26 = ((Gtk.Box.BoxChild)(this.vbox2[this.frame1]));
            w26.Position = 0;
            w26.Expand = false;
            w26.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.theme_configuration_container = new Gtk.Alignment(0.5F, 0.5F, 1F, 1F);
            this.theme_configuration_container.Name = "theme_configuration_container";
            this.vbox2.Add(this.theme_configuration_container);
            Gtk.Box.BoxChild w27 = ((Gtk.Box.BoxChild)(this.vbox2[this.theme_configuration_container]));
            w27.PackType = ((Gtk.PackType)(1));
            w27.Position = 1;
            // Container child vbox2.Gtk.Box+BoxChild
            this.composite_warning_widget = new Gtk.VBox();
            this.composite_warning_widget.Name = "composite_warning_widget";
            this.composite_warning_widget.Spacing = 6;
            // Container child composite_warning_widget.Gtk.Box+BoxChild
            this.label1 = new Gtk.Label();
            this.label1.Name = "label1";
            this.label1.LabelProp = Mono.Unix.Catalog.GetString("<b>Your display is not properly configured for theme and animation support. To use these features, you must enable compositing.</b>");
            this.label1.UseMarkup = true;
            this.label1.Wrap = true;
            this.composite_warning_widget.Add(this.label1);
            Gtk.Box.BoxChild w28 = ((Gtk.Box.BoxChild)(this.composite_warning_widget[this.label1]));
            w28.Position = 0;
            w28.Expand = false;
            w28.Fill = false;
            // Container child composite_warning_widget.Gtk.Box+BoxChild
            this.hbuttonbox1 = new Gtk.HButtonBox();
            this.hbuttonbox1.LayoutStyle = ((Gtk.ButtonBoxStyle)(4));
            // Container child hbuttonbox1.Gtk.ButtonBox+ButtonBoxChild
            this.composite_warning_info_btn = new Gtk.Button();
            this.composite_warning_info_btn.CanFocus = true;
            this.composite_warning_info_btn.Name = "composite_warning_info_btn";
            this.composite_warning_info_btn.UseStock = true;
            this.composite_warning_info_btn.UseUnderline = true;
            this.composite_warning_info_btn.Label = "gtk-dialog-info";
            this.hbuttonbox1.Add(this.composite_warning_info_btn);
            Gtk.ButtonBox.ButtonBoxChild w29 = ((Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox1[this.composite_warning_info_btn]));
            w29.Expand = false;
            w29.Fill = false;
            this.composite_warning_widget.Add(this.hbuttonbox1);
            Gtk.Box.BoxChild w30 = ((Gtk.Box.BoxChild)(this.composite_warning_widget[this.hbuttonbox1]));
            w30.Position = 1;
            w30.Expand = false;
            w30.Fill = false;
            this.vbox2.Add(this.composite_warning_widget);
            Gtk.Box.BoxChild w31 = ((Gtk.Box.BoxChild)(this.vbox2[this.composite_warning_widget]));
            w31.PackType = ((Gtk.PackType)(1));
            w31.Position = 2;
            this.Add(this.vbox2);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.composite_warning_info_btn.Hide();
            this.composite_warning_widget.Hide();
            this.Show();
            this.theme_combo.Changed += new System.EventHandler(this.OnThemeComboChanged);
            this.background_colorbutton.ColorSet += new System.EventHandler(this.OnBackgroundColorbuttonColorSet);
            this.clear_background.Clicked += new System.EventHandler(this.OnClearBackgroundClicked);
            this.pin_check.Clicked += new System.EventHandler(this.OnPinCheckClicked);
            this.shadow_check.Clicked += new System.EventHandler(this.OnShadowCheckClicked);
            this.animation_check.Clicked += new System.EventHandler(this.OnAnimationCheckbuttonClicked);
            this.composite_warning_info_btn.Clicked += new System.EventHandler(this.OnCompositeWarningInfoBtnClicked);
        }
    }
}
