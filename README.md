
# PHỤC HỒI CƠ SỞ DỮ LIỆU VỀ THỜI ĐIỂM BẤT KỲ TRÊN SQL SERVER 
## MỤC TIÊU:
Các DB khi phục hồi thường phải dựa vào 1 bản sao lưu cơ sở dữ liệu đã được thiết lập trước.   Vấn đề đặt ra ở đây là khi có sự cố trên cơ sở dữ liệu (thời điểm t2) mà bản sao lưu mới nhất  lại cách thời điểm đó vài tuần (thời điểm `t1` , với `t1< t2` ) thì làm cách nào ta có thể phục hồi cơ sở dữ liệu về thời điểm t trước khi xảy ra sự cố(`t1 < t < t2`).

### Các DB trên SQL Server được lưu trong `sys.sysdatabases`
```
SELECT        name, database_id
	FROM      sys.databases
	WHERE    (database_id >= 5) AND (NOT (name LIKE N'ReportServer%'))
	ORDER BY NAME
```
 `Backup device` : ta có thể sao lưu mỗi cơ sở dữ liệu vào 1 device ; Device là 1 file, nó cho phép sao lưu các bản backup của 1 cơ sở dữ liệu cùng vào 1 file, và về sau này nếu muốn phục hồi cơ sở dữ liệu, ta chỉ việc chọn bản backup đã sao lưu, lấy số thứ tự của bản backup đó, tiến hành phục hồi.
```
SELECT name  FROM sys.backup_devices
```

### Thông tin các bản backup được lưu trong :
- `msdb.dbo.Backupset`
- `msdb.dbo.backupmediafamily`

### Lệnh Backup cơ sở dữ liệu vào device:
```
BACKUP DATABASE  tenCSDL  TO tendevice [WITH INIT]
```

### LẤY POSITION CỦA CÁC BẢN BACKUP TRONG CSDL
```
SELECT     position, name, backup_start_date , user_name FROM  msdb.dbo.backupset 
   WHERE     database_name =@DBNAME AND type='D' AND 
     backup_set_id >= 
        (  SELECT  MAX(backup_set_id) 
        FROM msdb.dbo.backupset  
         WHERE database_name = @DBNAME AND type='D'
                  AND position=1  
        )
    ORDER BY position DESC

```
## PHỤC HỒI CƠ SỞ DỮ LIỆU VỀ THỜI ĐIỂM POSITION ĐÃ SAO LƯU
Tiến trình thực hiện:
```
> ALTER DATABASE TenDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE

> USE tempdb

> RESTORE DATABASE TenDB FROM  tendevice  WITH FILE= position, REPLACE

> ALTER DATABASE TenDB  SET MULTI_USER
```

### ĐIỀU KIỆN ĐỂ KHÔI PHỤC CƠ SỞ DỮ LIỆU VỀ THỜI ĐIỂM CHƯA SAO LƯU

- Database có `RECOVERY MODE` là `FULL`
- Database đã từng được `FULL BACKUP` ít nhất 1 lần
- File log chưa từng bị `SHRINK` kể từ sau lần full backup gần nhất

> CÁC BƯỚC THỰC HIỆN PHỤC HỒI

1. Đóng lại tất cả các kết nối đến database để không tiếp nhận thêm dữ liệu
2. Ghi lại thời điểm xảy ra lỗi
3. Thực hiện `BACKUP LOG` cho database
4. Khôi phục lại database theo trình tự sau:
+ `RESTORE` từ bản full backup mới nhất trước khi xảy ra sự cố
+ `RESTORE` từ bản log backup với lựa chọn `STOPAT` = thời điểm ngay trước khi có sự cố
5. Chuyển database sang chế độ hoạt động bình thường



## CÁC CHỨC NĂNG CỦA CHƯƠNG TRÌNH!

- Tạo backup device cho DB muốn sao lưu.

- Sao lưu cơ sở dữ liệu

- Phục hồi DB về thời điểm đã sao lưu

- Phục hồi DB về thời điểm chưa sao lưu
