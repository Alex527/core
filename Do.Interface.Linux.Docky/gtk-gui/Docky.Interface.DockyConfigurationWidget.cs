// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace Docky.Interface {
    
    
    public partial class DockyConfigurationWidget {
        
        private Gtk.VBox vbox2;
        
        private Gtk.Frame frame1;
        
        private Gtk.Alignment GtkAlignment;
        
        private Gtk.VBox vbox3;
        
        private Gtk.Table table1;
        
        private Gtk.CheckButton advanced_indicators_checkbutton;
        
        private Gtk.ComboBox autohide_combo;
        
        private Gtk.HBox hbox1;
        
        private Gtk.HBox hbox10;
        
        private Gtk.HBox hbox11;
        
        private Gtk.CheckButton zoom_checkbutton;
        
        private Gtk.HBox hbox4;
        
        private Gtk.HScale icon_size_scale;
        
        private Gtk.Label label2;
        
        private Gtk.ComboBox orientation_combobox;
        
        private Gtk.Label orientation_label;
        
        private Gtk.Label orientation_label1;
        
        private Gtk.HScale zoom_scale;
        
        private Gtk.Label GtkLabel3;
        
        private Gtk.HSeparator hseparator1;
        
        private Gtk.Frame frame2;
        
        private Gtk.Alignment GtkAlignment1;
        
        private Gtk.Table table2;
        
        private Gtk.HBox hbox6;
        
        private Gtk.HBox hbox7;
        
        private Gtk.HBox hbox8;
        
        private Gtk.Button clear_removed_button;
        
        private Gtk.HBox hbox9;
        
        private Gtk.ScrolledWindow scrolled_window;
        
        private Gtk.Label GtkLabel5;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget Docky.Interface.DockyConfigurationWidget
            Stetic.BinContainer.Attach(this);
            this.Name = "Docky.Interface.DockyConfigurationWidget";
            // Container child Docky.Interface.DockyConfigurationWidget.Gtk.Container+ContainerChild
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
            this.GtkAlignment.LeftPadding = ((uint)(12));
            this.GtkAlignment.TopPadding = ((uint)(5));
            this.GtkAlignment.RightPadding = ((uint)(5));
            this.GtkAlignment.BottomPadding = ((uint)(5));
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            this.vbox3 = new Gtk.VBox();
            this.vbox3.Name = "vbox3";
            this.vbox3.Spacing = 6;
            // Container child vbox3.Gtk.Box+BoxChild
            this.table1 = new Gtk.Table(((uint)(4)), ((uint)(5)), false);
            this.table1.Name = "table1";
            this.table1.RowSpacing = ((uint)(6));
            this.table1.ColumnSpacing = ((uint)(6));
            // Container child table1.Gtk.Table+TableChild
            this.advanced_indicators_checkbutton = new Gtk.CheckButton();
            this.advanced_indicators_checkbutton.CanFocus = true;
            this.advanced_indicators_checkbutton.Name = "advanced_indicators_checkbutton";
            this.advanced_indicators_checkbutton.Label = Mono.Unix.Catalog.GetString("Indicate Multiple Windows");
            this.advanced_indicators_checkbutton.DrawIndicator = true;
            this.advanced_indicators_checkbutton.UseUnderline = true;
            this.table1.Add(this.advanced_indicators_checkbutton);
            Gtk.Table.TableChild w1 = ((Gtk.Table.TableChild)(this.table1[this.advanced_indicators_checkbutton]));
            w1.LeftAttach = ((uint)(3));
            w1.RightAttach = ((uint)(4));
            w1.XOptions = ((Gtk.AttachOptions)(4));
            w1.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.autohide_combo = Gtk.ComboBox.NewText();
            this.autohide_combo.Name = "autohide_combo";
            this.table1.Add(this.autohide_combo);
            Gtk.Table.TableChild w2 = ((Gtk.Table.TableChild)(this.table1[this.autohide_combo]));
            w2.TopAttach = ((uint)(1));
            w2.BottomAttach = ((uint)(2));
            w2.LeftAttach = ((uint)(1));
            w2.RightAttach = ((uint)(2));
            w2.XOptions = ((Gtk.AttachOptions)(4));
            w2.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.hbox1 = new Gtk.HBox();
            this.hbox1.Name = "hbox1";
            this.hbox1.Spacing = 6;
            this.table1.Add(this.hbox1);
            Gtk.Table.TableChild w3 = ((Gtk.Table.TableChild)(this.table1[this.hbox1]));
            w3.LeftAttach = ((uint)(2));
            w3.RightAttach = ((uint)(3));
            w3.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.hbox10 = new Gtk.HBox();
            this.hbox10.Name = "hbox10";
            this.hbox10.Spacing = 6;
            // Container child hbox10.Gtk.Box+BoxChild
            this.hbox11 = new Gtk.HBox();
            this.hbox11.Name = "hbox11";
            this.hbox11.Spacing = 6;
            this.hbox10.Add(this.hbox11);
            Gtk.Box.BoxChild w4 = ((Gtk.Box.BoxChild)(this.hbox10[this.hbox11]));
            w4.Position = 1;
            // Container child hbox10.Gtk.Box+BoxChild
            this.zoom_checkbutton = new Gtk.CheckButton();
            this.zoom_checkbutton.CanFocus = true;
            this.zoom_checkbutton.Name = "zoom_checkbutton";
            this.zoom_checkbutton.Label = Mono.Unix.Catalog.GetString("Zoom:");
            this.zoom_checkbutton.DrawIndicator = true;
            this.zoom_checkbutton.UseUnderline = true;
            this.hbox10.Add(this.zoom_checkbutton);
            Gtk.Box.BoxChild w5 = ((Gtk.Box.BoxChild)(this.hbox10[this.zoom_checkbutton]));
            w5.Position = 2;
            w5.Expand = false;
            this.table1.Add(this.hbox10);
            Gtk.Table.TableChild w6 = ((Gtk.Table.TableChild)(this.table1[this.hbox10]));
            w6.TopAttach = ((uint)(3));
            w6.BottomAttach = ((uint)(4));
            w6.XOptions = ((Gtk.AttachOptions)(4));
            w6.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.hbox4 = new Gtk.HBox();
            this.hbox4.Name = "hbox4";
            this.hbox4.Spacing = 6;
            this.table1.Add(this.hbox4);
            Gtk.Table.TableChild w7 = ((Gtk.Table.TableChild)(this.table1[this.hbox4]));
            w7.LeftAttach = ((uint)(4));
            w7.RightAttach = ((uint)(5));
            w7.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.icon_size_scale = new Gtk.HScale(null);
            this.icon_size_scale.CanFocus = true;
            this.icon_size_scale.Name = "icon_size_scale";
            this.icon_size_scale.Adjustment.Lower = 24;
            this.icon_size_scale.Adjustment.Upper = 128;
            this.icon_size_scale.Adjustment.PageIncrement = 10;
            this.icon_size_scale.Adjustment.StepIncrement = 1;
            this.icon_size_scale.DrawValue = true;
            this.icon_size_scale.Digits = 0;
            this.icon_size_scale.ValuePos = ((Gtk.PositionType)(1));
            this.table1.Add(this.icon_size_scale);
            Gtk.Table.TableChild w8 = ((Gtk.Table.TableChild)(this.table1[this.icon_size_scale]));
            w8.TopAttach = ((uint)(2));
            w8.BottomAttach = ((uint)(3));
            w8.LeftAttach = ((uint)(1));
            w8.RightAttach = ((uint)(4));
            w8.XOptions = ((Gtk.AttachOptions)(4));
            w8.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label2 = new Gtk.Label();
            this.label2.Name = "label2";
            this.label2.Xalign = 1F;
            this.label2.LabelProp = Mono.Unix.Catalog.GetString("Icon Size:");
            this.table1.Add(this.label2);
            Gtk.Table.TableChild w9 = ((Gtk.Table.TableChild)(this.table1[this.label2]));
            w9.TopAttach = ((uint)(2));
            w9.BottomAttach = ((uint)(3));
            w9.XOptions = ((Gtk.AttachOptions)(4));
            w9.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.orientation_combobox = Gtk.ComboBox.NewText();
            this.orientation_combobox.Name = "orientation_combobox";
            this.table1.Add(this.orientation_combobox);
            Gtk.Table.TableChild w10 = ((Gtk.Table.TableChild)(this.table1[this.orientation_combobox]));
            w10.LeftAttach = ((uint)(1));
            w10.RightAttach = ((uint)(2));
            w10.XOptions = ((Gtk.AttachOptions)(4));
            w10.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.orientation_label = new Gtk.Label();
            this.orientation_label.WidthRequest = 70;
            this.orientation_label.Name = "orientation_label";
            this.orientation_label.Xalign = 1F;
            this.orientation_label.LabelProp = Mono.Unix.Catalog.GetString("Orientation:");
            this.table1.Add(this.orientation_label);
            Gtk.Table.TableChild w11 = ((Gtk.Table.TableChild)(this.table1[this.orientation_label]));
            w11.XOptions = ((Gtk.AttachOptions)(4));
            w11.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.orientation_label1 = new Gtk.Label();
            this.orientation_label1.Name = "orientation_label1";
            this.orientation_label1.Xalign = 1F;
            this.orientation_label1.LabelProp = Mono.Unix.Catalog.GetString("Hiding:");
            this.table1.Add(this.orientation_label1);
            Gtk.Table.TableChild w12 = ((Gtk.Table.TableChild)(this.table1[this.orientation_label1]));
            w12.TopAttach = ((uint)(1));
            w12.BottomAttach = ((uint)(2));
            w12.XOptions = ((Gtk.AttachOptions)(4));
            w12.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.zoom_scale = new Gtk.HScale(null);
            this.zoom_scale.CanFocus = true;
            this.zoom_scale.Name = "zoom_scale";
            this.zoom_scale.UpdatePolicy = ((Gtk.UpdateType)(1));
            this.zoom_scale.Adjustment.Lower = 0.1;
            this.zoom_scale.Adjustment.Upper = 4;
            this.zoom_scale.Adjustment.PageIncrement = 0.1;
            this.zoom_scale.Adjustment.PageSize = 0.1;
            this.zoom_scale.Adjustment.StepIncrement = 0.1;
            this.zoom_scale.Adjustment.Value = 0.7;
            this.zoom_scale.DrawValue = true;
            this.zoom_scale.Digits = 0;
            this.zoom_scale.ValuePos = ((Gtk.PositionType)(1));
            this.table1.Add(this.zoom_scale);
            Gtk.Table.TableChild w13 = ((Gtk.Table.TableChild)(this.table1[this.zoom_scale]));
            w13.TopAttach = ((uint)(3));
            w13.BottomAttach = ((uint)(4));
            w13.LeftAttach = ((uint)(1));
            w13.RightAttach = ((uint)(4));
            w13.XOptions = ((Gtk.AttachOptions)(4));
            w13.YOptions = ((Gtk.AttachOptions)(4));
            this.vbox3.Add(this.table1);
            Gtk.Box.BoxChild w14 = ((Gtk.Box.BoxChild)(this.vbox3[this.table1]));
            w14.Position = 0;
            w14.Expand = false;
            w14.Fill = false;
            this.GtkAlignment.Add(this.vbox3);
            this.frame1.Add(this.GtkAlignment);
            this.GtkLabel3 = new Gtk.Label();
            this.GtkLabel3.Name = "GtkLabel3";
            this.GtkLabel3.LabelProp = Mono.Unix.Catalog.GetString("<b>Docky Behavior</b>");
            this.GtkLabel3.UseMarkup = true;
            this.frame1.LabelWidget = this.GtkLabel3;
            this.vbox2.Add(this.frame1);
            Gtk.Box.BoxChild w17 = ((Gtk.Box.BoxChild)(this.vbox2[this.frame1]));
            w17.Position = 0;
            w17.Expand = false;
            w17.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.hseparator1 = new Gtk.HSeparator();
            this.hseparator1.Name = "hseparator1";
            this.vbox2.Add(this.hseparator1);
            Gtk.Box.BoxChild w18 = ((Gtk.Box.BoxChild)(this.vbox2[this.hseparator1]));
            w18.Position = 1;
            w18.Expand = false;
            w18.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.frame2 = new Gtk.Frame();
            this.frame2.Name = "frame2";
            this.frame2.ShadowType = ((Gtk.ShadowType)(0));
            // Container child frame2.Gtk.Container+ContainerChild
            this.GtkAlignment1 = new Gtk.Alignment(0F, 0F, 1F, 1F);
            this.GtkAlignment1.Name = "GtkAlignment1";
            this.GtkAlignment1.LeftPadding = ((uint)(12));
            this.GtkAlignment1.TopPadding = ((uint)(5));
            this.GtkAlignment1.RightPadding = ((uint)(12));
            // Container child GtkAlignment1.Gtk.Container+ContainerChild
            this.table2 = new Gtk.Table(((uint)(2)), ((uint)(3)), false);
            this.table2.Name = "table2";
            this.table2.RowSpacing = ((uint)(6));
            this.table2.ColumnSpacing = ((uint)(6));
            // Container child table2.Gtk.Table+TableChild
            this.hbox6 = new Gtk.HBox();
            this.hbox6.WidthRequest = 70;
            this.hbox6.Name = "hbox6";
            this.hbox6.Spacing = 6;
            this.table2.Add(this.hbox6);
            Gtk.Table.TableChild w19 = ((Gtk.Table.TableChild)(this.table2[this.hbox6]));
            w19.XOptions = ((Gtk.AttachOptions)(4));
            // Container child table2.Gtk.Table+TableChild
            this.hbox7 = new Gtk.HBox();
            this.hbox7.Name = "hbox7";
            this.hbox7.Spacing = 6;
            this.table2.Add(this.hbox7);
            Gtk.Table.TableChild w20 = ((Gtk.Table.TableChild)(this.table2[this.hbox7]));
            w20.LeftAttach = ((uint)(2));
            w20.RightAttach = ((uint)(3));
            w20.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table2.Gtk.Table+TableChild
            this.hbox8 = new Gtk.HBox();
            this.hbox8.Name = "hbox8";
            this.hbox8.Spacing = 6;
            // Container child hbox8.Gtk.Box+BoxChild
            this.clear_removed_button = new Gtk.Button();
            this.clear_removed_button.CanFocus = true;
            this.clear_removed_button.Name = "clear_removed_button";
            this.clear_removed_button.UseUnderline = true;
            // Container child clear_removed_button.Gtk.Container+ContainerChild
            Gtk.Alignment w21 = new Gtk.Alignment(0.5F, 0.5F, 0F, 0F);
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            Gtk.HBox w22 = new Gtk.HBox();
            w22.Spacing = 2;
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Image w23 = new Gtk.Image();
            w23.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gtk-clear", Gtk.IconSize.Menu, 16);
            w22.Add(w23);
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Label w25 = new Gtk.Label();
            w25.LabelProp = Mono.Unix.Catalog.GetString("Reinstate Removed Items");
            w25.UseUnderline = true;
            w22.Add(w25);
            w21.Add(w22);
            this.clear_removed_button.Add(w21);
            this.hbox8.Add(this.clear_removed_button);
            Gtk.Box.BoxChild w29 = ((Gtk.Box.BoxChild)(this.hbox8[this.clear_removed_button]));
            w29.Position = 0;
            w29.Expand = false;
            w29.Fill = false;
            // Container child hbox8.Gtk.Box+BoxChild
            this.hbox9 = new Gtk.HBox();
            this.hbox9.Name = "hbox9";
            this.hbox9.Spacing = 6;
            this.hbox8.Add(this.hbox9);
            Gtk.Box.BoxChild w30 = ((Gtk.Box.BoxChild)(this.hbox8[this.hbox9]));
            w30.Position = 2;
            this.table2.Add(this.hbox8);
            Gtk.Table.TableChild w31 = ((Gtk.Table.TableChild)(this.table2[this.hbox8]));
            w31.TopAttach = ((uint)(1));
            w31.BottomAttach = ((uint)(2));
            w31.LeftAttach = ((uint)(1));
            w31.RightAttach = ((uint)(2));
            w31.XOptions = ((Gtk.AttachOptions)(4));
            w31.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table2.Gtk.Table+TableChild
            this.scrolled_window = new Gtk.ScrolledWindow();
            this.scrolled_window.CanFocus = true;
            this.scrolled_window.Name = "scrolled_window";
            this.scrolled_window.ShadowType = ((Gtk.ShadowType)(1));
            this.table2.Add(this.scrolled_window);
            Gtk.Table.TableChild w32 = ((Gtk.Table.TableChild)(this.table2[this.scrolled_window]));
            w32.LeftAttach = ((uint)(1));
            w32.RightAttach = ((uint)(2));
            this.GtkAlignment1.Add(this.table2);
            this.frame2.Add(this.GtkAlignment1);
            this.GtkLabel5 = new Gtk.Label();
            this.GtkLabel5.Name = "GtkLabel5";
            this.GtkLabel5.LabelProp = Mono.Unix.Catalog.GetString("<b>Docklets</b>");
            this.GtkLabel5.UseMarkup = true;
            this.frame2.LabelWidget = this.GtkLabel5;
            this.vbox2.Add(this.frame2);
            Gtk.Box.BoxChild w35 = ((Gtk.Box.BoxChild)(this.vbox2[this.frame2]));
            w35.Position = 2;
            w35.Expand = false;
            w35.Fill = false;
            this.Add(this.vbox2);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.Hide();
            this.zoom_scale.FormatValue += new Gtk.FormatValueHandler(this.OnZoomScaleFormatValue);
            this.zoom_scale.ValueChanged += new System.EventHandler(this.OnZoomScaleValueChanged);
            this.orientation_combobox.Changed += new System.EventHandler(this.OnOrientationComboboxChanged);
            this.icon_size_scale.ValueChanged += new System.EventHandler(this.OnIconSizeScaleValueChanged);
            this.zoom_checkbutton.Toggled += new System.EventHandler(this.OnZoomCheckbuttonToggled);
            this.autohide_combo.Changed += new System.EventHandler(this.OnAutohideComboChanged);
            this.advanced_indicators_checkbutton.Toggled += new System.EventHandler(this.OnAdvancedIndicatorsCheckbuttonToggled);
            this.clear_removed_button.Clicked += new System.EventHandler(this.OnClearRemovedButtonClicked);
        }
    }
}
