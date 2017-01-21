using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaLib
{
    namespace UI
    {
        static class ListViewItemWithEnable
        {
            public static void enable(this ListViewItem listItem, bool enable)
            {
                if (enable == true)
                {
                    listItem.ForeColor = System.Drawing.Color.Black;
                }
                else
                {
                    listItem.ForeColor = System.Drawing.Color.Gray;
                }
            }
        }

        public class ListViewItemComparer : IComparer
        {
            private int col;
            private bool direct;
            public ListViewItemComparer(int _c, bool _d) { col = _c; direct = _d; }
            public int Compare(object x, object y)
            {
                int returnVal = -1;
                returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
                if (direct == false) returnVal *= -1;
                return returnVal;
            }
        }

        delegate void ImgProcTask();
        public delegate bool MediaFilter(Lib.Media media);

        public class MediaListViewHelper
        {
            static MediaListViewHelper helper = new MediaListViewHelper();
            MediaListViewHelper() { }
            static public MediaListViewHelper instance { get { return helper; } }
            ListView mediaListView;
            ImageList largeImageList;
            ImageList smallImageList;
            Config.ListViewConfig config;
            Dictionary<String,ListViewItem> allItems; 

            public MediaFilter viewfilter;

            int listViewCreatedThread;
            public void setListView(ListView listView) {
                mediaListView = listView;
                listViewCreatedThread = System.Threading.Thread.CurrentThread.ManagedThreadId;
                Resource.ImgManager.loadConfigs();

                //largeImgaeList
                largeImageList = new ImageList();
                largeImageList.ImageSize = new System.Drawing.Size(200, 150);
                largeImageList.ColorDepth = ColorDepth.Depth32Bit;
                largeImageList.Images.AddRange(Resource.ImgManager.getDefaultImageList().ToArray());

                //smallImageList
                smallImageList = new ImageList();
                smallImageList.ImageSize = new System.Drawing.Size(200/3, 150/3);
                smallImageList.ColorDepth = ColorDepth.Depth32Bit;
                smallImageList.Images.AddRange(Resource.ImgManager.getDefaultImageList().ToArray());


                mediaListView.SmallImageList = smallImageList;
                mediaListView.LargeImageList = largeImageList;

                //set up data
                allItems = new Dictionary<string, ListViewItem>();
                //mediaListView.MouseWheel += MediaListView_MouseWheel;
                mediaListView.SelectedIndexChanged += MediaListView_SelectedIndexChanged;
                
                mediaListView.VirtualMode = true;
                
                mediaListView.RetrieveVirtualItem += MediaListView_RetrieveVirtualItem;
                //mediaListView.CacheVirtualItems += MediaListView_CacheVirtualItems;
            }

            

            List<String> displayList;
            Dictionary<String, int> imgIndex = new Dictionary<string, int>();
            Dictionary<String,ListViewItem> displayCache = new Dictionary<string, ListViewItem>();
            Dictionary<String, Lib.Media> allMedia = new Dictionary<string, Lib.Media>();

            //load image for list in backgroud
            BackgroundWorker loadImageWorker = new BackgroundWorker();
            ThreadQueue loadImageQueue = new ThreadQueue();

           
            
            private void safeRefresh()
            {
                if (System.Threading.Thread.CurrentThread.ManagedThreadId == listViewCreatedThread)
                {
                    mediaListView.VirtualListSize = displayList.Count();
                    mediaListView.Refresh();
                }
                else
                {
                    Action<Object> AsyncUIDelegate = delegate (Object o)
                    {
                        mediaListView.VirtualListSize = displayList.Count();
                        mediaListView.Refresh();
                    };

                    while (!mediaListView.IsHandleCreated) ;
                    mediaListView.Invoke(AsyncUIDelegate, new object[] { displayList });
                }
            }
            

            public void reloadVirtualData()
            {
                //copy to avoid muti-thread conflict
                allMedia.Clear();
                Lib.MediaLib.instance.TravelMedium((Lib.Media media) => {
                    allMedia.Add(media.UID, media);
                });
                displayList = buildDisplayList();
                safeRefresh();

            }

            public void loadImgs(IEnumerable<Lib.Media> medius)
            {
                foreach (Lib.Media media in medius)
                {
                    loadImageQueue.enqueueTask(new ThreadQueue.Task(() => {
                        loadImageForMedia(media);
                    }));
                }
            }

            private void loadImageForMedia(Lib.Media media)
            {
                if (!imgIndex.ContainsKey(media.UID))
                {
                    media.imgMgr.getImageFromMedia(media,
                   (System.Drawing.Image image, Lib.Media _media) =>
                   {
                       System.Drawing.Image displayImage = new System.Drawing.Bitmap(200, 150);
                       var graphics = System.Drawing.Graphics.FromImage(displayImage);
                       var size = image.Size;
                       double ratio = Math.Min(200.0 / (double)image.Width, 150.0 / (double)image.Height);
                       size.Height = (int)(ratio * size.Height);
                       size.Width = (int)(ratio * size.Width);
                       if (200.0 / (double)image.Width < 150.0 / (double)image.Height)
                           graphics.DrawImage(image, new System.Drawing.Rectangle(new System.Drawing.Point(0, 75 - size.Height / 2), size));
                       else
                           graphics.DrawImage(image, new System.Drawing.Rectangle(new System.Drawing.Point(100 - size.Width / 2, 0), size));
                       graphics.DrawImage(media.imgMgr.defaultImage, new System.Drawing.Rectangle(120, 73, 200 / 2, 150 / 2));
                       graphics.Dispose();
                       Func<System.Drawing.Image, System.Drawing.Image, Lib.Media,bool> AsyncUIDelegate1 = updateImgForItemVritual;
                       mediaListView.Invoke(AsyncUIDelegate1, new object[] { displayImage, image, media });
                   });
                }
            }

            List<String> buildDisplayList()
            {
                if (viewfilter == null)
                {
                    return  allMedia.Keys.ToList();
                }
                else
                {
                    return allMedia.Where((KeyValuePair<String, Lib.Media> item) =>
                    {
                        return viewfilter(item.Value);
                    }).Select((KeyValuePair<String, Lib.Media> item) =>
                    {
                        return item.Key;
                    }).ToList();
                    
                }
            }
          
            private void setUpListViewItem(Lib.Media media, ListViewItem lvi)
            {
                // title fixed
                lvi.Text = media.title;
                lvi.Name = media.UID;
                lvi.enable(media.enable);
                foreach (var headerConfig in config.Headers)
                {
                    if (headerConfig.Text != "Title")
                    {
                        lvi.SubItems.Add(media[headerConfig.DataMap]);
                    }
                }
            }
            Lib.Media getMediaFromIndex(int index)
            {
                Lib.Media media = null;
                try
                {
                    media = allMedia[displayList[index]];
                }
                catch (Exception ex) { Logger.ERROR(ex.Message); }
                return media;
            }
            //virtual mode
            private void MediaListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
            {
                if (e.ItemIndex >= displayList.Count) {
                    e.Item = new ListViewItem();
                    return;
                }
                ListViewItem lvi = null;
                String uid = displayList[e.ItemIndex];
                Lib.Media media = allMedia[uid];
                //already has it
                if (displayCache.ContainsKey(uid))
                {
                    lvi = displayCache[uid];
                    e.Item = lvi;
                }
                else
                {
                    lvi = new ListViewItem();
                    displayCache[uid] = lvi;
                    //fill the item
                    setUpListViewItem(media, lvi);
                    e.Item = lvi;
                }

                if (lvi.ImageIndex == -1)
                {
                    lvi.ImageIndex = media.imgMgr.defaultImgIndex;// avoid to execute it again, default image
                }
                else if (lvi.ImageIndex == media.imgMgr.defaultImgIndex && imgIndex.ContainsKey(media.UID))
                {
                    try
                    {
                        lvi.ImageIndex = imgIndex[media.UID];
                    }
                    catch(Exception ex)
                    {
                        Logger.ERROR(ex.Message);
                    }
                }
                    


            }
            
            //events
            //avoid to select a unenable item.
            private void MediaListView_SelectedIndexChanged(object sender, EventArgs e)
            {
                foreach(int  index in mediaListView.SelectedIndices)
                {
                    Lib.Media media = getMediaFromIndex(index);
                    if (media == null || media.enable == false)
                        displayCache[media.UID].Selected = false;
                }
            }

            //zoom in & out & give up
            private void MediaListView_MouseWheel(object sender, MouseEventArgs e)
            {
               
                double zoom;
                if (e.Delta > 0)
                {
                    zoom = 1.1;
                }
                else
                {
                    zoom = 0.9;
                }
                System.Drawing.Size imgSize = mediaListView.LargeImageList.ImageSize;
                imgSize.Height = (int)(imgSize.Height * zoom);
                imgSize.Width = (int)(imgSize.Width * zoom);

                if (imgSize.Width > 256)
                {
                    imgSize.Width = 256;
                    imgSize.Height = 144;
                }
                if(imgSize.Width < 30)
                {
                    imgSize.Width = 30;
                    imgSize.Height = 17;
                }
                List<System.Drawing.Image> tmp = new List<System.Drawing.Image>();
                foreach(System.Drawing.Image img in mediaListView.LargeImageList.Images)
                {
                    tmp.Add(img);
                }
                mediaListView.LargeImageList.ImageSize = imgSize;
              foreach(System.Drawing.Image image in tmp)
                {
                    mediaListView.LargeImageList.Images.Add(image);
                }


            }

            public void changeView(View view)
            {
                mediaListView.View = view;
            }

            //initail
            public bool loadList()
            {
                //load configuration
                config = Config.ConfigHelper<Config.ListViewConfig>.instance.Config("MediaList");
                if (config == null)
                    return false;

                setListHeader();

                //load media
                // list will be loaded by rootMangers
                Lib.MediaLib.instance.initRootManagersFromLocal();                               
                return true;
            }

            //only start loading data after handle created
           
            private void setListHeader()
            {
                foreach(var headerConfig in config.Headers)
                {
                    ColumnHeader ch = new ColumnHeader();
                    ch.Text = headerConfig.Text;
                    ch.Width = headerConfig.Width;
                    ch.TextAlign = HorizontalAlignment.Left;
                    mediaListView.Columns.Add(ch);
                }
            }

            /*
             * 
             private void updateItemInMainThread(Lib.Media media)
            {
                ListViewItem lvi;
                if (allItems.ContainsKey(media.UID)){
                    lvi = allItems[media.UID];
                    if(mediaListView.Items.ContainsKey(media.UID)==false)
                        mediaListView.Items.Add(lvi);
                }
                else
                {
                    lvi = new ListViewItem();
                    allItems.Add(media.UID, lvi);
                    mediaListView.Items.Add(lvi);
                }
                // title fixed
                lvi.Text = media.title;
                lvi.Name = media.UID;
                lvi.enable(media.enable);
                foreach (var headerConfig in config.Headers)
                {
                    if (headerConfig.Text != "Title")
                    {
                        lvi.SubItems.Add(media[headerConfig.DataMap]);
                    }
                }

                if (media.enable == false) return;
                lvi.ImageIndex = media.imgMgr.defaultImgIndex;// avoid to execute it again, default image
                if (media.imgMgr == null) return;
                media.imgMgr.getImageFromMedia(media,
                (System.Drawing.Image image, Lib.Media _media) =>
                {
                    System.Drawing.Image displayImage = new System.Drawing.Bitmap(200, 150);
                    var graphics = System.Drawing.Graphics.FromImage(displayImage);
                    var size = image.Size;
                    double ratio = Math.Min( 200.0/(double)image.Width , 150.0/ (double)image.Height);
                    size.Height= (int )(ratio* size.Height);
                    size.Width = (int)(ratio * size.Width);
                    if(200.0 / (double)image.Width< 150.0 / (double)image.Height )
                        graphics.DrawImage(image, new System.Drawing.Rectangle(new System.Drawing.Point(0,75 - size.Height/2), size));
                    else
                        graphics.DrawImage(image, new System.Drawing.Rectangle(new System.Drawing.Point(100- size.Width/2, 0), size));
                    graphics.DrawImage(media.imgMgr.defaultImage, new System.Drawing.Rectangle(120, 73, 200/2, 150/2));
                    
                    Action<Object> AsyncUIDelegate = delegate (Object o)
                    {
                        updateImgForItem(lvi, displayImage, image, _media);
                    };

                    while (!mediaListView.IsHandleCreated) ;
                    mediaListView.Invoke(AsyncUIDelegate, new object[] { media });
                });

            }

            //reload everything
            public void reloadData()
            {
                mediaListView.Items.Clear();
                mediaListView.BeginUpdate();
                //
                Lib.MediaLib.instance.TravelMedium((Lib.Media media) => {
                    updateItemInMainThread(media);
                });
                mediaListView.EndUpdate();

            }

                //only  refresh items in order to implement filter
            public void refreshItems()
            {
                mediaListView.Items.Clear();
                mediaListView.BeginUpdate();
                //on need to refresh, recover all items
                if (viewfilter == null)
                {
                    mediaListView.Items.AddRange(allItems.Values.ToArray());
                    mediaListView.EndUpdate();
                    return;
                }             
                Lib.MediaLib.instance.TravelMedium((Lib.Media media) => {
                    //if has a filter
                    if (viewfilter(media))
                        mediaListView.Items.Add(allItems[media.UID]);

                });
                mediaListView.EndUpdate();
            }


            private Lib.Media getMediaFromItem(ListViewItem item)
            {
                String UID = item.Name;
                return Lib.MediaLib.instance.getMedia(UID);
            }

            private void updateImgForItem(ListViewItem item, System.Drawing.Image Largeimg, System.Drawing.Image smallimg, Lib.Media media)
            {
                    smallImageList.Images.Add(media.UID, smallimg);
                    largeImageList.Images.Add(media.UID, Largeimg);
                    item.ImageIndex = largeImageList.Images.IndexOfKey(media.UID);
                mediaListView.Refresh();
            }


            */
            private bool updateImgForItemVritual(System.Drawing.Image Largeimg, System.Drawing.Image smallimg, Lib.Media media)
            {
                smallImageList.Images.Add(media.UID, smallimg);
                largeImageList.Images.Add(media.UID, Largeimg);
                imgIndex[media.UID] = largeImageList.Images.IndexOfKey(media.UID);
                mediaListView.Refresh();
                return true;
            }

           

            

            //event handle
            public void doubleClick(int index)
            {
                Lib.Media media = getMediaFromIndex(index);
                if(media.enable)
                    System.Diagnostics.Process.Start("explorer.exe", "\""+media.contentDir+"\"");
            }

            Dictionary<int, bool> columSortInfo = new Dictionary<int, bool>();

            

            public void reSort(int sortColum)
            {

                if (!columSortInfo.ContainsKey(sortColum))
                    columSortInfo[sortColum] = false;

                columSortInfo[sortColum] = !columSortInfo[sortColum];
                displayList.Sort(new Comparison<String>((String a, String b) => {
                    ListViewItem ai = displayCache[a];
                    ListViewItem bi = displayCache[b];
                    int returnVal = String.Compare(ai.SubItems[sortColum].Text, bi.SubItems[sortColum].Text);
                    if (columSortInfo[sortColum] == false) returnVal *= -1;
                    return returnVal;
                }));
                mediaListView.Refresh();
            }


            //other thread can access to update ui
            //background thread change to  main thread
            public void updateItem(Lib.Media media)
            {
                //update info
                //remove from cache
                if (displayCache.ContainsKey(media.UID))
                {
                    displayCache.Remove(media.UID);
                }
                //virtual mode
                reloadVirtualData();
                loadImgs(new List<Lib.Media>() { media});
                
            }

            public void updateItems(IEnumerable<Lib.Media> medium)
            {
                foreach (Lib.Media media in medium)
                {
                    //update info
                    //remove from cache
                    if (displayCache.ContainsKey(media.UID))
                    {
                        displayCache.Remove(media.UID);
                    }
                }
                reloadVirtualData();
                loadImgs(medium);
            }

            public void removeItems(IEnumerable<Lib.Media> medium)
            {
                foreach(Lib.Media media in medium)
                {
                    ListViewItem lvi = mediaListView.Items[media.UID];
                    //release Image by set to default image;
                    largeImageList.Images[lvi.ImageIndex] = media.imgMgr.defaultImage;              
                }
                reloadVirtualData();
            }
            public void removeItem(Lib.Media media)
            {
                //virtual mode remove
                ListViewItem lvi = mediaListView.Items[media.UID];
                if (lvi == null) return;
                //release Image by set to default image;
                largeImageList.Images[lvi.ImageIndex] = media.imgMgr.defaultImage;
                reloadVirtualData();
            }
        }



        public class DoubleBufferListView : System.Windows.Forms.ListView
        {
            public DoubleBufferListView()
            {
                SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
                UpdateStyles();
            }
        }
    }
 }
