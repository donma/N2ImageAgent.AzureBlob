# N2Image Agent for Azure Blob

這是一套基於 .net Core 2.2.103 + Azure Storage Blob , 這主要功能可以幫你把圖放到 Blob 上面 ，你可以輕易透過
http://yourdomain.com/source/[IMAGE_ID] 得到你的圖片

如果您想要圖片圖片變成 寬度100高度隨比例縮放，只需要透過 http://yourdomain.com/image/[IMAGE_ID]/100 即可取得

如果您想要圖片圖片變成 高度ㄉ00寬度隨比例縮放，只需要透過 http://yourdomain.com/image/[IMAGE_ID]/0/100 即可取得

值得一提，您可以限定使用者下載的秒數，預設是 90 秒鐘，使用者可以有下載該圖片的權限

您甚至可以從 http://yourdomain.com/info/[IMAGE_ID] 得到該圖片的原始資訊像是這樣


Image Info
```json
{"Id":"01d1z3nrvgzhyde8qc9dvjg88e","Width":5120,"Height":2466,"Extension":"jpeg","Tag":"測試"}
```

Link
----
* [NuGet Gallery | LevelUp.Serializer.JsonNet](https://www.nuget.org/packages/LevelUp.Serializer.JsonNet/)
