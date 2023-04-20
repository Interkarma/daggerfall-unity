#ifndef __XAMARIN_GETIFADDRS_H

#include "monodroid.h"

#ifdef __cplusplus
extern "C" {
#endif

/* We're implementing getifaddrs behavior, this is the structure we use. It is exactly the same as
 * struct ifaddrs defined in ifaddrs.h but since bionics doesn't have it we need to mirror it here.
 */
struct _monodroid_ifaddrs
{
    struct _monodroid_ifaddrs *ifa_next; /* Pointer to the next structure.      */

    char *ifa_name;                      /* Name of this network interface.     */
    unsigned int ifa_flags;              /* Flags as from SIOCGIFFLAGS ioctl.   */

    struct sockaddr *ifa_addr;           /* Network address of this interface.  */
    struct sockaddr *ifa_netmask;        /* Netmask of this interface.          */
    union
    {
        /* At most one of the following two is valid.  If the IFF_BROADCAST
           bit is set in `ifa_flags', then `ifa_broadaddr' is valid.  If the
           IFF_POINTOPOINT bit is set, then `ifa_dstaddr' is valid.
           It is never the case that both these bits are set at once.  */
        struct sockaddr *ifu_broadaddr;  /* Broadcast address of this interface. */
        struct sockaddr *ifu_dstaddr;    /* Point-to-point destination address.  */
    } ifa_ifu;
    /* These very same macros are defined by <net/if.h> for `struct ifaddr'.
       So if they are defined already, the existing definitions will be fine.  */
# ifndef _monodroid_ifa_broadaddr
#  define _monodroid_ifa_broadaddr ifa_ifu.ifu_broadaddr
# endif
# ifndef _monodroid_ifa_dstaddr
#  define _monodroid_ifa_dstaddr   ifa_ifu.ifu_dstaddr
# endif

    void *ifa_data;               /* Address-specific data (may be unused).  */
};

void _monodroid_getifaddrs_init(void);
MONO_API  int  _monodroid_getifaddrs(struct _monodroid_ifaddrs **ifap);
MONO_API  void _monodroid_freeifaddrs(struct _monodroid_ifaddrs *ifa);

#ifdef __cplusplus
}
#endif

#endif
