/* Copyright (c) 2009 InMon Corp. ALL RIGHTS RESERVED */
/* License: http://www.inmon.com/products/virtual-probe/license.php */

#if defined(__cplusplus)
extern "C" {
#endif

#include "hsflowd.h"

#define HSP_MAX_LINELEN 2048
#define HSP_MAX_CONFIG_DEPTH 3
#define HSP_SEPARATORS " \t\r\n=;"

extern int debug;
 
#define ADD_TO_LIST(linkedlist, obj) \
  do { \
    obj->nxt = linkedlist; \
    linkedlist = obj; \
  } while(0)
    

  static HSPSFlowSettings *newSFlowSettings(HSPSFlow *sf) {
    HSPSFlowSettings *st = (HSPSFlowSettings *)my_calloc(sizeof(HSPSFlowSettings));
    st->pollingInterval = SFL_DEFAULT_POLLING_INTERVAL;
    return st;
  }

  static HSPSFlow *newSFlow(HSP *sp) {
    HSPSFlow *sf = (HSPSFlow *)my_calloc(sizeof(HSPSFlow));
    sf->sFlowSettings = newSFlowSettings(sf);
    sf->subAgentId = 0;
    sp->sFlow = sf; // just one of these, not a list
    sf->myHSP = sp;
    return sf;
  }

  static HSPCollector *newCollector(HSPSFlow *sf) {
    HSPCollector *col = (HSPCollector *)my_calloc(sizeof(HSPCollector));
    ADD_TO_LIST(sf->collectors, col);
    sf->numCollectors++;
    col->udpPort = SFL_DEFAULT_COLLECTOR_PORT;
    return col;
  }


  int HSPReadConfig(HSP *sp)
  {
	HSPCollector *col;
	DWORD dwRet,cbData = 16;
	HKEY hkey;
	char collector_ip[16];
	int gotData = NO;
	struct sockaddr_in *sendSocketAddr;

	newSFlow(sp);
	
	// just take the winning agent-address
	sp->sFlow->agentIP = sp->agentIP;
	sp->sFlow->agentDevice = my_strdup(sp->agentDevice);
	sp->sFlow->ipPriority = sp->ipPriority;

	newCollector(sp->sFlow);
	col = sp->sFlow->collectors;


	dwRet = RegOpenKeyEx(HKEY_LOCAL_MACHINE,
						"system\\currentcontrolset\\services\\hsflowd\\Parameters",
						0,
						KEY_QUERY_VALUE,
						&hkey);
	if(dwRet != ERROR_SUCCESS) return gotData;

	memset(collector_ip,0,16);
	dwRet = RegQueryValueEx( hkey,
                             "collector",
                             NULL,
                             NULL,
                             (LPBYTE) collector_ip,
                             &cbData );
	if(dwRet != ERROR_SUCCESS) return gotData;

	gotData = YES;
	col->ipAddr.address.ip_v4.addr = inet_addr(collector_ip);
	col->ipAddr.type = SFLADDRESSTYPE_IP_V4;
	sendSocketAddr = (struct sockaddr_in *)&col->sendSocketAddr;
	sendSocketAddr->sin_family = AF_INET;
	sendSocketAddr->sin_addr.s_addr = col->ipAddr.address.ip_v4.addr;

	DWORD dwPort = 0;
	dwRet = RegQueryValueEx( hkey,
                             "port",
                             NULL,
                             NULL,
                             (LPBYTE)&dwPort,
                             &cbData );
	if(dwRet == ERROR_SUCCESS) {
		if (dwPort <= 0 || dwPort > 65535) {
			myLog(debug, "readConfig: invalid sFlow udp port %u read from system\\currentcontrolset\\services\\hsflowd\\Parameters\\port, using default %u",
				dwPort, SFL_DEFAULT_COLLECTOR_PORT);
		} else {
			col->udpPort = dwPort;
		}
	}

	DWORD dwPollingInterval = 0;
	dwRet = RegQueryValueEx(hkey,
							"pollingInterval",
							NULL,
							NULL,
							(LPBYTE)&dwPollingInterval,
							&cbData);
	if (dwRet == ERROR_SUCCESS) {
		sp->sFlow->sFlowSettings->pollingInterval = dwPollingInterval;
	}
	RegCloseKey(hkey);
	
	myLog(LOG_INFO,"collector: %s port: %u pollingInterval: %u",
		collector_ip, col->udpPort, sp->sFlow->sFlowSettings->pollingInterval);

    return gotData;
  }

#if defined(__cplusplus)
} /* extern "C" */
#endif
