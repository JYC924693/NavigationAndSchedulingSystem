#if !defined(_INCLUDE_H_)
#define _INCLUDE_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifdef PUBLIC_EXPORT
#define MONTION_CONTROL_EXPORT AFX_EXT_CLASS
#else
#define MONTION_CONTROL_EXPORT AFX_CLASS_IMPORT
#endif

enum MONTION_CONTROL_EXPORT SoftElemType
{
	//AM600
	ELEM_QX = 0,     //QXԪ��
	ELEM_MW = 1,     //MWԪ��
	ELEM_X = 2,		 //XԪ��(��ӦQX200~QX300)
	ELEM_Y = 3,		 //YԪ��(��ӦQX300~QX400)

	//H3U
	REGI_H3U_Y    = 0x20,       //YԪ���Ķ���	
	REGI_H3U_X    = 0x21,		//XԪ���Ķ���							
	REGI_H3U_S    = 0x22,		//SԪ���Ķ���				
	REGI_H3U_M    = 0x23,		//MԪ���Ķ���							
	REGI_H3U_TB   = 0x24,		//TλԪ���Ķ���				
	REGI_H3U_TW   = 0x25,		//T��Ԫ���Ķ���				
	REGI_H3U_CB   = 0x26,		//CλԪ���Ķ���				
	REGI_H3U_CW   = 0x27,		//C��Ԫ���Ķ���				
	REGI_H3U_DW   = 0x28,		//D��Ԫ���Ķ���				
	REGI_H3U_CW2  = 0x29,	    //C˫��Ԫ���Ķ���
	REGI_H3U_SM   = 0x2a,		//SM
	REGI_H3U_SD   = 0x2b,		//
	REGI_H3U_R    = 0x2c,		//
	//H5u
	REGI_H5U_Y    = 0x30,       //YԪ���Ķ���	
	REGI_H5U_X    = 0x31,		//XԪ���Ķ���							
	REGI_H5U_S    = 0x32,		//SԪ���Ķ���				
	REGI_H5U_M    = 0x33,		//MԪ���Ķ���	
	REGI_H5U_B    = 0x34,       //BԪ���Ķ���
	REGI_H5U_D    = 0x35,       //D��Ԫ���Ķ���
	REGI_H5U_R    = 0x36,       //R��Ԫ���Ķ���
};
enum MONTION_CONTROL_EXPORT BatchElemType
{
	//H3U
	BAT_H3U_Y    = 0,       //YԪ���Ķ���	
	BAT_H3U_X    = 1,		//XԪ���Ķ���							
	BAT_H3U_S    = 2,		//SԪ���Ķ���				
	BAT_H3U_M    = 3,		//MԪ���Ķ���							
	BAT_H3U_TB   = 4,		//TλԪ���Ķ���				
	BAT_H3U_TW   = 5,		//T��Ԫ���Ķ���				
	BAT_H3U_CB   = 6,		//CλԪ���Ķ���				
	BAT_H3U_CW   = 7,		//C��Ԫ���Ķ���				
	BAT_H3U_DW   = 8,		//D��Ԫ���Ķ���				
	BAT_H3U_CW2  = 9,	    //C˫��Ԫ���Ķ���
	BAT_H3U_SM   = 10,		//SM
	BAT_H3U_SD   = 11,		//
	BAT_H3U_R    = 12,		//	
	BAT_H3U_DW2  = 13,		//D˫��Ԫ���Ķ���
	BAT_H3U_SDW2 = 14,      //SD˫��Ԫ���Ķ���
	BAT_H3U_RW2  = 15,      //R˫��Ԫ���Ķ���	
	//H5U
	BAT_H5U_Y    = 16,       //YԪ���Ķ���	
	BAT_H5U_X    = 17,		//XԪ���Ķ���							
	BAT_H5U_S    = 18,		//SԪ���Ķ���
	BAT_H5U_B    = 19,		//SԪ���Ķ���
	BAT_H5U_M    = 20,		//MԪ���Ķ���
	BAT_H5U_D    = 21,		//DԪ���Ķ���	
	BAT_H5U_R    = 22,		//RԪ���Ķ���		
};

enum MONTION_CONTROL_EXPORT ErrorCode
{
	ER_READ_WRITE_FAIL =0,   //��дʧ��
	ER_READ_WRITE_SUCCEED =1,  //��д�ɹ�
	ER_NOT_CONNECT =2,  //δ����
	ER_ELEM_TYPE_WRONG =3,  //Ԫ�����ʹ���
	ER_ELEM_ADDR_OVER =4,  //Ԫ����ַ���
	ER_ELEM_COUNT_OVER =5,  //Ԫ����������
	ER_COMM_EXCEPT =6,  //ͨѶ�쳣
};

struct MONTION_CONTROL_EXPORT BatchRegInfo
{
	BatchElemType  enElemType;    //�Ĵ�������
	long           nAddr;         //�Ĵ�����ַ
	long           nValue;        //�Ĵ���ֵ������
};

struct H3uAddrTypeInfo
{
	DWORD dwAddrStart;		//��ʼ��ַ
	DWORD dwAddrPlc;		//��ʼ��ַ��Ӧ��PLC��ַ
	DWORD dwAddrLen;		//��ַ�ܵĸ���
	DWORD dwAddrType;		//��ַ���ʹ�����ֽ���, 0λ���ͣ�2��WORD�ͣ�4��˫����
};

static H3uAddrTypeInfo g_dwH3uReadAddr[]={
	{0, 0xfc00, 255, 0},		//YԪ���Ķ���
	{0, 0xf800, 255, 0},		//XԪ���Ķ���
	{0, 0xe000, 4095, 0},		//SԪ���Ķ���
	{0, 0x000, 8511, 0},		//MԪ���Ķ���
	{0, 0xf000, 511, 0},		//TλԪ���Ķ���
	{0, 0xf000, 511, 2},		//T��Ԫ���Ķ���
	{0, 0xf400, 255, 0},		//CλԪ���Ķ���	
	{0, 0xf400, 199, 2},		//C��Ԫ���Ķ���
	{0, 0x0000, 8511, 2},		//D��Ԫ���Ķ���	
	{200, 0xf700, 55, 4},		//C˫��Ԫ���Ķ���
	{0, 0x2400, 1023, 0},		//SM	
	{0, 0x2400, 1023, 2},		//SD
	{0, 0x3000, 32767, 2},		//RԪ���Ķ���
	{0, 0x0000, 8510, 4},		//D˫��Ԫ���Ķ���	
	{0, 0x2400, 1022, 4},		//SD
	{0, 0x3000, 32766, 4},		//RԪ���Ķ���

	{0, 0, 0,	 0},
};

static H3uAddrTypeInfo g_dwAm600ReadAddr[]={
	{0, 0, 81917, 0},		//qx
	{0, 0, 65535, 2},		//mw
	{0, 1600, 99*8+7, 0},		//x
	{0, 2400, 99*8+7, 0},		//y
	{0, 0, 0,	 0},
};

static H3uAddrTypeInfo g_dwH3uBitWriteAddr[]={
	{0, 0xfc00, 255, 0},		//YԪ���Ķ���
	{0, 0xf800, 255, 0},		//XԪ���Ķ���
	{0, 0xe000, 4095, 0},		//SԪ���Ķ���
	{0, 0x000, 8511, 0},		//MԪ���Ķ���
	{0, 0xf000, 511, 0},		//TλԪ���Ķ���
	{0, 0xf000, 511, 2},		//T��Ԫ���Ķ���
	{0, 0xf400, 255, 0},		//CλԪ���Ķ���	
	{0, 0xf400, 199, 2},		//C��Ԫ���Ķ���
	{0, 0x0000, 8511, 2},		//D��Ԫ���Ķ���	
	{200, 0xf700, 55, 4},		//C˫��Ԫ���Ķ���
	{0, 0x2400, 1023, 0},		//SM	
	{0, 0x2400, 1023, 2},		//SD
	{0, 0x3000, 32767, 2},		//RԪ���Ķ���
	{0, 0, 0,	 0},
};

static H3uAddrTypeInfo g_dwWiteBitAddr_H3u[]={
	{5000, 10, 0},		
	{5100, 10, 0},		
	{5200, 10, 0},		
	{5300, 10, 0},		
	{5400, 10, 0},		
	{5500, 10, 0},		
	{5600, 10, 0},		
	{5700, 10, 0},		
	//{5800, 10, 0},		
	//{5900, 10, 0},		
	{6100, 10, 0},		
	{6200, 10, 0},		
	{6300, 10, 0},		
	{6400, 10, 0},		
	//{6500, 10, 0},	
	{0, 0,	 0},
};

static H3uAddrTypeInfo g_dwH5uReadAddr[]={
	{0, 0xfc00, 1023, 0},		//YԪ���Ķ���
	{0, 0xf800, 1023, 0},		//XԪ���Ķ���
	{0, 0xe000, 4095, 0},		//SԪ���Ķ���
	{0, 0x000, 7999, 0},		//MԪ���Ķ���
	{0,0x3000,32767,0},         //BԪ���Ķ���
	{0,0x0000,7999,2},          //D��Ԫ���Ķ���
	{0,0x3000,32767,2},         //R��Ԫ���Ķ���
	{0, 0, 0,0},
};
#define AXIS_NUM_ADDR 1000

#define MODBUSTCP_RD_COIL_MAX                       1968                        //����Ȧ�������
#define MODBUSTCP_WR_COIL_MAX                       1936                        //д��Ȧ�������
#define MODBUSTCP_RD_REG_MAX                        123                        //���Ĵ����������
#define MODBUSTCP_WR_REG_MAX                        121                        //д�Ĵ����������


#endif	//	#if !defined(_INCLUDE_H_)