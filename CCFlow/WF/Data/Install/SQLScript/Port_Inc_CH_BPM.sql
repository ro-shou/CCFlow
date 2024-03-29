﻿-- 执行删除.
DELETE FROM Port_Dept;
DELETE FROM Port_Station;
DELETE FROM Port_Emp;
DELETE FROM Port_DeptEmpStation;
DELETE FROM Port_DeptEmp;
DELETE FROM Port_StationType;
 
 
-- Port_Dept ;
DELETE FROM Port_Dept;
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('100','グループ本部','0')   ;
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('1001','グループマーケティング部門','100')  ;
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('1002','グループ研究開発部','100')  ;
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('1003','グループサービス部門','100')  ;
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('1004','グループ財務部','100')  ;
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('1005','グループ人事部','100') ;
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('1060','南方支店','100') ;
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('1061',   'マーケティング部','1060') ;
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('1062',    '財務部','1060') ;
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('1063',    '販売部','1060') ;

INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('1070','北方支店','100') ;
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('1071','マーケティング部','1070') ;
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('1072','財務部','1070') ;
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('1073','販売部','1070') ;
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('1099','外来単位','100')  ;


-- Port_StationType ;
DELETE FROM Port_StationType;
INSERT INTO Port_StationType (No,Name) VALUES('1','高層');
INSERT INTO Port_StationType (No,Name) VALUES('2','中層');
INSERT INTO Port_StationType (No,Name) VALUES('3','基層');
 
-- Port_Station ;
DELETE FROM Port_Station;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('01','ゼネラルマネージャー','1','0') ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('02','マーケティング・マネージャー','2','0');
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('03','R&Dマネージャー','2','0')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('04','カスタマーサービスマネージャー','2','0')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('05','財務マネージャー','2','0')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('06','人事マネージャー','2','0')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('07','営業スタッフの投稿','3','0')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('08','プログラマーの投稿','3','0')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('09','テクニカルサービスポスト','3','0')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('10','キャッシャーポスト','3','0')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('11','人事アシスタントポスト','3','0')  ;
INSERT INTO Port_Station (No,Name,FK_StationType,OrgNo) VALUES('12','外来人員役職','3','0')  ;



-- Port_Emp ;
-- 总经理部 ;
DELETE FROM Port_Emp;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email) VALUES('admin','admin','123','100','0531-82374939','zhoupeng@ccflow.org')  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email) VALUES('zhoupeng','周鵬','123','100','0531-82374939','zhoupeng@ccflow.org')  ;

-- 市场部 ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email) VALUES('zhanghaicheng','張海成','123','1001','0531-82374939','zhanghaicheng@ccflow.org')  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email) VALUES('zhangyifan','張一帆','123','1001','0531-82374939','zhangyifan@ccflow.org')  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email) VALUES('zhoushengyu','周昇雨','123','1001','0531-82374939','zhoushengyu@ccflow.org')  ;

-- 研发部 ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email) VALUES('qifenglin','チーフェンリン','123','1002','0531-82374939','qifenglin@ccflow.org')  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email) VALUES('zhoutianjiao','周天嬌','123','1002','0531-82374939','zhoutianjiao@ccflow.org')  ;

-- 服务部经理 ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email) VALUES('guoxiangbin','郭翔斌','123','1003','0531-82374939','guoxiangbin@ccflow.org')  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email) VALUES('fuhui','福恵','123','1003','0531-82374939','fuhui@ccflow.org')  ;

-- 财务部 ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email) VALUES('yangyilei','楊依雷','123','1004','0531-82374939','yangyilei@ccflow.org')  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email) VALUES('guobaogeng','郭宝源','123','1004','0531-82374939','guobaogeng@ccflow.org') ;

-- 人力资源部 ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email) VALUES('liping','李平','123','1005','0531-82374939','liping@ccflow.org')  ;
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email) VALUES('liyan','李燕','123','1005','0531-82374939','liyan@ccflow.org')  ;

-- 外来单位人员
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept,Tel,Email) VALUES('Guest','外来人員','123','1099','0531-82374939','Guest@ccflow.org')  ;

 


--==== 增加部门与岗位对应.;

DELETE FROM Port_DeptStation;

INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('100', '01');
   -- 市场部
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1001','02');  
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1001','07');  
   -- 研发部
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1002','03');  
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1002','08'); 
   -- 客服部
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1003','04');  
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1003','09');  
   -- 财务部
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1004','05');  
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1004','10');  
   -- 人力资源
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1005','06');  
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1005','11');

-- 外来单位
INSERT INTO Port_DeptStation (FK_Dept,FK_Station) VALUES ('1099','12');
  
 
-- Port_DeptEmp 人员与部门的对应 ;
DELETE FROM Port_DeptEmp;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('100_zhoupeng','zhoupeng','100') ;

-- 市场部 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('1001_zhanghaicheng','zhanghaicheng','1001') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('1001_zhangyifan','zhangyifan','1001') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('1001_zhoushengyu','zhoushengyu','1001') ;

-- 研发部 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('1002_qifenglin','qifenglin','1002') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('1002_zhoutianjiao','zhoutianjiao','1002') ;

-- 服务部经理 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('1003_guoxiangbin','guoxiangbin','1003') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('1003_fuhui',            'fuhui','1003') ;

-- 财务部 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('1004_yangyilei','yangyilei','1004') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('1004_guobaogeng','guobaogeng','1004') ;

-- 人力资源部 ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('1005_liping','liping','1005') ;
INSERT INTO Port_DeptEmp (MyPK,FK_Emp,FK_Dept) VALUES('1005_liyan','liyan','1005') ;

-- Port_DeptEmpStation 人员与岗位的对应 ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('100_zhoupeng_01','100','zhoupeng','01')  ;

-- 市场部; 
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1001_zhanghaicheng_02','1001','zhanghaicheng','02')  ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1001_zhangyifan_07','1001','zhangyifan','07')  ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1001_zhoushengyu_07','1001','zhoushengyu','07')  ;

-- 研发部 ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1002_qifenglin_03','1002','qifenglin','03')  ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1002_zhoutianjiao_08','1002','zhoutianjiao','08')  ;

--服务部;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1003_guoxiangbin_04','1003','guoxiangbin','04');
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1003_fuhui_09',      '1003','fuhui','09') ; 

-- 财务部;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1004_yangyilei_05','1004','yangyilei','05')   ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1004_guobaogeng_10','1004','guobaogeng','10')  ;

-- 人力资源部;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1005_liping_06','1005','liping','06')  ;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1005_liyan_11','1005','liyan','11')  ;

-- 外来单位人员;
INSERT INTO Port_DeptEmpStation (MyPK,FK_Dept,FK_Emp,FK_Station) VALUES('1099_Guest_12','1005','Guest','12') ;