using System;
using System.Collections.Generic;
using System.Text;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocolsNames
{
    public static class BluetoothServiceRegistration
    {
        // e.g.: given guid "0000fee7-0000-1000-8000-00805f9b34fb"
        // id is fee7 and return Tencent Holdings Limited.'s registration
        static Guid MasterGuid = Guid.Parse("0000fee7-0000-1000-8000-00805f9b34fb");
        public static Registration FindRegistration(ushort id)
        {
            if (id >= 0x1800 && id <= 0x180F)
            {
                return StandardRegistration;
            }
            var name = GetMember_Uuid(id);
            if (name == null) return null;
            return new Registration(id, name, "3/3/2023");

            // Old code. New code is automatically generated from YAML files
            //foreach (var item in Registrations)
            //{
            //    if (item.Id == id) return item;
            //}
            //return null;
        }
        public static Registration FindRegistration(Guid g)
        {
            var bytes = g.ToByteArray();
            var mgb = MasterGuid.ToByteArray();
            for (int i = 0; i < 16; i++)
            {
                if (i >= 0 && i < 2) continue;
                if (bytes[i] != mgb[i]) return null;
            }
            //ushort us = (ushort)(bytes[2] << 8) + (ushort)bytes[3];
            ushort hi = (ushort)(bytes[1] << 8);
            ushort lo = (ushort)bytes[0];
            ushort us = (ushort)(hi + lo);
            return FindRegistration(us);
        }
        public static Registration FindRegistration(string str)
        {
            Guid g;
            bool parseOk = Guid.TryParse(str, out g);
            if (!parseOk) return null;
            return FindRegistration(g);
        }
        public class Registration
        {
            public Registration(ushort id, string registrationOwner, string registrationDate)
            {
                Id = id;
                RegistrationOwner = registrationOwner;
                RegistrationDate = registrationDate;
            }
            public override string ToString()
            {
                return $"{Id:X2} from {RegistrationOwner}";
            }
            public ushort Id { get; set; }
            public string RegistrationOwner { get; set; }
            /// <summary>
            /// The registration date is no longer supplied with the YAML file
            /// </summary>
            private string RegistrationDate { get; set; }
        }

        private static Registration StandardRegistration = new Registration(0x180F, "Bluetooth Standard", "1/1/1970");

        // 2026: or look in the YAML file: https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/uuids/member_uuids.yaml

        private static string GetMember_Uuid(uint uuid)
        {
            switch (uuid)
            {
                // updatefile:
                // url:https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/uuids/member_uuids.yaml
                // file:member_uuids.yaml
                // startupdatefile:
				case 0xFEFF: return "GN Netcom"; // 
				case 0xFEFE: return "GN Hearing A/S"; // 
				case 0xFEFD: return "Gimbal, Inc."; // 
				case 0xFEFC: return "Gimbal, Inc."; // 
				case 0xFEFB: return "Telit Wireless Solutions (Formerly Stollmann E+V GmbH)"; // 
				case 0xFEFA: return "PayPal, Inc."; // 
				case 0xFEF9: return "PayPal, Inc."; // 
				case 0xFEF8: return "Aplix Corporation"; // 
				case 0xFEF7: return "Aplix Corporation"; // 
				case 0xFEF6: return "Wicentric, Inc."; // 
				case 0xFEF5: return "Dialog Semiconductor GmbH"; // 
				case 0xFEF4: return "Google LLC"; // 
				case 0xFEF3: return "Google LLC"; // 
				case 0xFEF2: return "CSR"; // 
				case 0xFEF1: return "CSR"; // 
				case 0xFEF0: return "Intel"; // 
				case 0xFEEF: return "Polar Electro Oy"; // 
				case 0xFEEE: return "Polar Electro Oy"; // 
				case 0xFEED: return "Tile, Inc."; // 
				case 0xFEEC: return "Tile, Inc."; // 
				case 0xFEEB: return "Swirl Networks, Inc."; // 
				case 0xFEEA: return "Swirl Networks, Inc."; // 
				case 0xFEE9: return "Quintic Corp."; // 
				case 0xFEE8: return "Quintic Corp."; // 
				case 0xFEE7: return "Tencent Holdings Limited."; // 
				case 0xFEE6: return "Silvair, Inc."; // 
				case 0xFEE5: return "Nordic Semiconductor ASA"; // 
				case 0xFEE4: return "Nordic Semiconductor ASA"; // 
				case 0xFEE3: return "Anki, Inc."; // 
				case 0xFEE2: return "Anki, Inc."; // 
				case 0xFEE1: return "Anhui Huami Information Technology Co., Ltd."; // 
				case 0xFEE0: return "Anhui Huami Information Technology Co., Ltd."; // 
				case 0xFEDE: return "Coin, Inc."; // 
				case 0xFEDD: return "Jawbone"; // 
				case 0xFEDC: return "Jawbone"; // 
				case 0xFEDB: return "Perka, Inc."; // 
				case 0xFEDA: return "ISSC Technologies Corp."; // 
				case 0xFED9: return "Pebble Technology Corporation"; // 
				case 0xFED8: return "Google LLC"; // 
				case 0xFED7: return "Broadcom"; // 
				case 0xFED6: return "Broadcom"; // 
				case 0xFED5: return "Plantronics Inc."; // 
				case 0xFED4: return "Apple, Inc."; // 
				case 0xFED3: return "Apple, Inc."; // 
				case 0xFED2: return "Apple, Inc."; // 
				case 0xFED1: return "Apple, Inc."; // 
				case 0xFED0: return "Apple, Inc."; // 
				case 0xFECF: return "Apple, Inc."; // 
				case 0xFECE: return "Apple, Inc."; // 
				case 0xFECD: return "Apple, Inc."; // 
				case 0xFECC: return "Apple, Inc."; // 
				case 0xFECB: return "Apple, Inc."; // 
				case 0xFECA: return "Apple, Inc."; // 
				case 0xFEC9: return "Apple, Inc."; // 
				case 0xFEC8: return "Apple, Inc."; // 
				case 0xFEC7: return "Apple, Inc."; // 
				case 0xFEC6: return "Kocomojo, LLC"; // 
				case 0xFEC5: return "Realtek Semiconductor Corp."; // 
				case 0xFEC4: return "PLUS Location Systems"; // 
				case 0xFEC3: return "360fly, Inc."; // 
				case 0xFEC2: return "Blue Spark Technologies, Inc."; // 
				case 0xFEC1: return "KDDI Corporation"; // 
				case 0xFEC0: return "KDDI Corporation"; // 
				case 0xFEBF: return "Nod, Inc."; // 
				case 0xFEBE: return "Bose Corporation"; // 
				case 0xFEBD: return "Clover Network, Inc"; // 
				case 0xFEBC: return "Dexcom Inc"; // 
				case 0xFEBB: return "adafruit industries"; // 
				case 0xFEBA: return "Tencent Holdings Limited"; // 
				case 0xFEB9: return "LG Electronics"; // 
				case 0xFEB8: return "Meta Platforms, Inc."; // 
				case 0xFEB7: return "Meta Platforms, Inc."; // 
				case 0xFEB6: return "Vencer Co., Ltd"; // 
				case 0xFEB5: return "WiSilica Inc."; // 
				case 0xFEB4: return "WiSilica Inc."; // 
				case 0xFEB3: return "Taobao"; // 
				case 0xFEB2: return "Microsoft Corporation"; // 
				case 0xFEB1: return "Electronics Tomorrow Limited"; // 
				case 0xFEB0: return "Nest Labs Inc"; // 
				case 0xFEAF: return "Nest Labs Inc"; // 
				case 0xFEAE: return "Nokia"; // 
				case 0xFEAD: return "Nokia"; // 
				case 0xFEAC: return "Nokia"; // 
				case 0xFEAB: return "Nokia"; // 
				case 0xFEAA: return "Google LLC"; // 
				case 0xFEA9: return "Savant Systems LLC"; // 
				case 0xFEA8: return "Savant Systems LLC"; // 
				case 0xFEA7: return "UTC Fire and Security"; // 
				case 0xFEA6: return "GoPro, Inc."; // 
				case 0xFEA5: return "GoPro, Inc."; // 
				case 0xFEA4: return "Paxton Access Ltd"; // 
				case 0xFEA3: return "ITT Industries"; // 
				case 0xFEA0: return "Google LLC"; // 
				case 0xFE9F: return "Google LLC"; // 
				case 0xFE9E: return "Renesas Design Netherlands B.V."; // 
				case 0xFE9D: return "Mobiquity Networks Inc"; // 
				case 0xFE9C: return "GSI Laboratories, Inc."; // 
				case 0xFE9B: return "Samsara Networks, Inc"; // 
				case 0xFE9A: return "Estimote"; // 
				case 0xFE99: return "Currant Inc"; // 
				case 0xFE98: return "Currant Inc"; // 
				case 0xFE97: return "Tesla Motors Inc."; // 
				case 0xFE96: return "Tesla Motors Inc."; // 
				case 0xFE95: return "Xiaomi Inc."; // 
				case 0xFE94: return "OttoQ In"; // 
				case 0xFE93: return "OttoQ In"; // 
				case 0xFE92: return "Jarden Safety & Security"; // 
				case 0xFE91: return "Shanghai Imilab Technology Co.,Ltd"; // 
				case 0xFE90: return "JUMA"; // 
				case 0xFE8F: return "CSR"; // 
				case 0xFE8E: return "ARM Ltd"; // 
				case 0xFE8D: return "Interaxon Inc."; // 
				case 0xFE8C: return "TRON Forum"; // 
				case 0xFE8B: return "Apple, Inc."; // 
				case 0xFE8A: return "Apple, Inc."; // 
				case 0xFE89: return "B&O Play A/S"; // 
				case 0xFE88: return "SALTO SYSTEMS S.L."; // 
				case 0xFE87: return "Qingdao Yeelink Information Technology Co., Ltd. ( 青岛亿联客信息技术有限公司 )"; // 
				case 0xFE86: return "HUAWEI Technologies Co., Ltd"; // 
				case 0xFE85: return "RF Digital Corp"; // 
				case 0xFE84: return "RF Digital Corp"; // 
				case 0xFE83: return "Blue Bite"; // 
				case 0xFE82: return "Medtronic Inc."; // 
				case 0xFE81: return "Medtronic Inc."; // 
				case 0xFE80: return "Doppler Lab"; // 
				case 0xFE7F: return "Doppler Lab"; // 
				case 0xFE7E: return "Awear Solutions Ltd"; // 
				case 0xFE7D: return "Aterica Health Inc."; // 
				case 0xFE7C: return "Telit Wireless Solutions (Formerly Stollmann E+V GmbH)"; // 
				case 0xFE7B: return "Orion Labs, Inc."; // 
				case 0xFE7A: return "Bragi GmbH"; // 
				case 0xFE79: return "Zebra Technologies"; // 
				case 0xFE78: return "Hewlett-Packard Company"; // 
				case 0xFE77: return "Hewlett-Packard Company"; // 
				case 0xFE76: return "TangoMe"; // 
				case 0xFE75: return "TangoMe"; // 
				case 0xFE74: return "unwire"; // 
				case 0xFE73: return "Abbott (formerly St. Jude Medical, Inc.)"; // 
				case 0xFE72: return "Abbott (formerly St. Jude Medical, Inc.)"; // 
				case 0xFE71: return "Plume Design Inc"; // 
				case 0xFE70: return "Beijing Jingdong Century Trading Co., Ltd."; // 
				case 0xFE6F: return "LINE Corporation"; // 
				case 0xFE6E: return "The University of Tokyo"; // 
				case 0xFE6D: return "The University of Tokyo"; // 
				case 0xFE6C: return "TASER International, Inc."; // 
				case 0xFE6B: return "TASER International, Inc."; // 
				case 0xFE6A: return "Kontakt Micro-Location Sp. z o.o."; // 
				case 0xFE69: return "Capsle Technologies Inc."; // 
				case 0xFE68: return "Capsle Technologies Inc."; // 
				case 0xFE67: return "Lab Sensor Solutions"; // 
				case 0xFE66: return "Intel Corporation"; // 
				case 0xFE65: return "CHIPOLO d.o.o."; // 
				case 0xFE64: return "Siemens AG"; // 
				case 0xFE63: return "Connected Yard, Inc."; // 
				case 0xFE62: return "Indagem Tech LLC"; // 
				case 0xFE61: return "Logitech International SA"; // 
				case 0xFE60: return "Lierda Science & Technology Group Co., Ltd."; // 
				case 0xFE5F: return "Eyefi, Inc."; // 
				case 0xFE5E: return "Plastc Corporation"; // 
				case 0xFE5D: return "Grundfos A/S"; // 
				case 0xFE5C: return "million hunters GmbH"; // 
				case 0xFE5B: return "GT-tronics HK Ltd"; // 
				case 0xFE5A: return "Cronologics Corporation"; // 
				case 0xFE59: return "Nordic Semiconductor ASA"; // 
				case 0xFE58: return "Nordic Semiconductor ASA"; // 
				case 0xFE57: return "Dotted Labs"; // 
				case 0xFE56: return "Google LLC"; // 
				case 0xFE55: return "Google LLC"; // 
				case 0xFE54: return "Motiv, Inc."; // 
				case 0xFE53: return "3M"; // 
				case 0xFE52: return "SetPoint Medical"; // 
				case 0xFE51: return "SRAM"; // 
				case 0xFE50: return "Google LLC"; // 
				case 0xFE4F: return "Molekule, Inc."; // 
				case 0xFE4E: return "NTT docomo"; // 
				case 0xFE4D: return "Casambi Technologies Oy"; // 
				case 0xFE4C: return "Volkswagen AG"; // 
				case 0xFE4B: return "Signify Netherlands B.V. (formerly Philips Lighting B.V.)"; // 
				case 0xFE4A: return "OMRON HEALTHCARE Co., Ltd."; // 
				case 0xFE49: return "SenionLab AB"; // 
				case 0xFE48: return "General Motors"; // 
				case 0xFE47: return "General Motors"; // 
				case 0xFE46: return "B&O Play A/S"; // 
				case 0xFE45: return "Snapchat Inc"; // 
				case 0xFE44: return "SK Telecom"; // 
				case 0xFE43: return "Andreas Stihl AG & Co. KG"; // 
				case 0xFE42: return "Nets A/S"; // 
				case 0xFE41: return "Inugo Systems Limited"; // 
				case 0xFE40: return "Inugo Systems Limited"; // 
				case 0xFE3F: return "Friday Labs Limited"; // 
				case 0xFE3E: return "BD Medical"; // 
				case 0xFE3D: return "BD Medical"; // 
				case 0xFE3C: return "alibaba"; // 
				case 0xFE3B: return "Dolby Laboratories"; // 
				case 0xFE3A: return "TTS Tooltechnic Systems AG & Co. KG"; // 
				case 0xFE39: return "TTS Tooltechnic Systems AG & Co. KG"; // 
				case 0xFE36: return "HUAWEI Technologies Co., Ltd"; // 
				case 0xFE35: return "HUAWEI Technologies Co., Ltd"; // 
				case 0xFE34: return "SmallLoop LLC"; // 
				case 0xFE33: return "CHIPOLO d.o.o."; // 
				case 0xFE32: return "Pro-Mark, Inc."; // 
				case 0xFE31: return "Volkswagen AG"; // 
				case 0xFE30: return "Volkswagen AG"; // 
				case 0xFE2F: return "CRESCO Wireless, Inc"; // 
				case 0xFE2E: return "ERi,Inc."; // 
				case 0xFE2D: return "LAMPLIGHT Co., Ltd."; // 
				case 0xFE2C: return "Google LLC"; // 
				case 0xFE2B: return "ITT Industries"; // 
				case 0xFE2A: return "DaisyWorks, Inc."; // 
				case 0xFE29: return "Gibson Innovations"; // 
				case 0xFE28: return "Ayla Networks"; // 
				case 0xFE27: return "Google LLC"; // 
				case 0xFE26: return "Google LLC"; // 
				case 0xFE25: return "Apple, Inc."; // 
				case 0xFE24: return "August Home Inc"; // 
				case 0xFE23: return "Zoll Medical Corporation"; // 
				case 0xFE22: return "Zoll Medical Corporation"; // 
				case 0xFE21: return "Bose Corporation"; // 
				case 0xFE20: return "Emerson"; // 
				case 0xFE1F: return "Garmin International, Inc."; // 
				case 0xFE1E: return "LAMPLIGHT Co., Ltd."; // 
				case 0xFE1D: return "Illuminati Instrument Corporation"; // 
				case 0xFE1C: return "NetMedia, Inc."; // 
				case 0xFE1B: return "Tyto Life LLC"; // 
				case 0xFE1A: return "Tyto Life LLC"; // 
				case 0xFE19: return "Google LLC"; // 
				case 0xFE18: return "Runtime, Inc."; // 
				case 0xFE17: return "Telit Wireless Solutions GmbH"; // 
				case 0xFE16: return "Footmarks, Inc."; // 
				case 0xFE15: return "Amazon.com Services, Inc.."; // 
				case 0xFE14: return "Flextronics International USA Inc."; // 
				case 0xFE13: return "Apple Inc."; // 
				case 0xFE12: return "M-Way Solutions GmbH"; // 
				case 0xFE11: return "GMC-I Messtechnik GmbH"; // 
				case 0xFE10: return "LAPIS Technology Co., Ltd."; // 
				case 0xFE0F: return "Signify Netherlands B.V. (formerly Philips Lighting B.V.)"; // 
				case 0xFE0E: return "Setec Pty Ltd"; // 
				case 0xFE0D: return "Procter & Gamble"; // 
				case 0xFE0C: return "Procter & Gamble"; // 
				case 0xFE0B: return "ruwido austria gmbh"; // 
				case 0xFE0A: return "ruwido austria gmbh"; // 
				case 0xFE09: return "Pillsy, Inc."; // 
				case 0xFE08: return "Microsoft"; // 
				case 0xFE07: return "Sonos, Inc."; // 
				case 0xFE06: return "Qualcomm Technologies, Inc."; // 
				case 0xFE05: return "CORE Transport Technologies NZ Limited"; // 
				case 0xFE04: return "Motorola Solutions, Inc."; // 
				case 0xFE03: return "Amazon.com Services, Inc."; // 
				case 0xFE02: return "Robert Bosch GmbH"; // 
				case 0xFE01: return "Duracell U.S. Operations Inc."; // 
				case 0xFE00: return "Amazon.com Services, Inc."; // 
				case 0xFDFF: return "OSRAM GmbH"; // 
				case 0xFDFE: return "ADHERIUM(NZ) LIMITED"; // 
				case 0xFDFD: return "RecursiveSoft Inc."; // 
				case 0xFDFC: return "Optrel AG"; // 
				case 0xFDFB: return "Tandem Diabetes Care"; // 
				case 0xFDFA: return "Tandem Diabetes Care"; // 
				case 0xFDF9: return "INIA"; // 
				case 0xFDF8: return "Onvocal"; // 
				case 0xFDF7: return "HP Inc."; // 
				case 0xFDF6: return "AIAIAI ApS"; // 
				case 0xFDF5: return "Milwaukee Electric Tools"; // 
				case 0xFDF4: return "O. E. M. Controls, Inc."; // 
				case 0xFDF3: return "Amersports"; // 
				case 0xFDF2: return "AMICCOM Electronics Corporation"; // 
				case 0xFDF1: return "LAMPLIGHT Co.,Ltd"; // 
				case 0xFDF0: return "Google LLC"; // 
				case 0xFDEF: return "ART AND PROGRAM, INC."; // 
				case 0xFDEE: return "Huawei Technologies Co., Ltd."; // 
				case 0xFDED: return "Pole Star"; // 
				case 0xFDEC: return "Mannkind Corporation"; // 
				case 0xFDEB: return "Syntronix Corporation"; // 
				case 0xFDEA: return "SeeScan, Inc"; // 
				case 0xFDE9: return "Spacesaver Corporation"; // 
				case 0xFDE8: return "Robert Bosch GmbH"; // 
				case 0xFDE7: return "SECOM Co., LTD"; // 
				case 0xFDE6: return "Intelletto Technologies Inc"; // 
				case 0xFDE5: return "SMK Corporation"; // 
				case 0xFDE4: return "JUUL Labs, Inc."; // 
				case 0xFDE3: return "Abbott Diabetes Care"; // 
				case 0xFDE2: return "Google LLC"; // 
				case 0xFDE1: return "Fortin Electronic Systems"; // 
				case 0xFDE0: return "John Deere"; // 
				case 0xFDDF: return "Harman International"; // 
				case 0xFDDE: return "Noodle Technology Inc."; // 
				case 0xFDDD: return "Arch Systems Inc"; // 
				case 0xFDDC: return "4iiii Innovations Inc."; // 
				case 0xFDDB: return "Samsung Electronics Co., Ltd."; // 
				case 0xFDDA: return "MHCS"; // 
				case 0xFDD9: return "Jiangsu Teranovo Tech Co., Ltd."; // 
				case 0xFDD8: return "Jiangsu Teranovo Tech Co., Ltd."; // 
				case 0xFDD7: return "Copeland Cold Chain LP"; // 
				case 0xFDD6: return "Ministry of Supply"; // 
				case 0xFDD5: return "Brompton Bicycle Ltd"; // 
				case 0xFDD4: return "LX Solutions Pty Limited"; // 
				case 0xFDD3: return "FUBA Automotive Electronics GmbH"; // 
				case 0xFDD2: return "Bose Corporation"; // 
				case 0xFDD1: return "Huawei Technologies Co., Ltd"; // 
				case 0xFDD0: return "Huawei Technologies Co., Ltd"; // 
				case 0xFDCF: return "Nalu Medical, Inc"; // 
				case 0xFDCE: return "SENNHEISER electronic GmbH & Co. KG"; // 
				case 0xFDCD: return "Qingping Technology (Beijing) Co., Ltd."; // 
				case 0xFDCC: return "Shoof Technologies"; // 
				case 0xFDCB: return "Meggitt SA"; // 
				case 0xFDCA: return "Fortin Electronic Systems"; // 
				case 0xFDC9: return "Busch-Jaeger Elektro GmbH"; // 
				case 0xFDC8: return "Hach – Danaher"; // 
				case 0xFDC7: return "Eli Lilly and Company"; // 
				case 0xFDC6: return "Eli Lilly and Company"; // 
				case 0xFDC5: return "Automatic Labs"; // 
				case 0xFDC4: return "Simavita (Aust) Pty Ltd"; // 
				case 0xFDC3: return "Baidu Online Network Technology (Beijing) Co., Ltd"; // 
				case 0xFDC2: return "Baidu Online Network Technology (Beijing) Co., Ltd"; // 
				case 0xFDC1: return "Hunter Douglas"; // 
				case 0xFDC0: return "Hunter Douglas"; // 
				case 0xFDBF: return "California Things Inc."; // 
				case 0xFDBE: return "California Things Inc."; // 
				case 0xFDBD: return "Clover Network, Inc."; // 
				case 0xFDBC: return "Emerson"; // 
				case 0xFDBB: return "Profoto"; // 
				case 0xFDB8: return "LivaNova USA Inc."; // 
				case 0xFDB7: return "LivaNova USA Inc."; // 
				case 0xFDB6: return "GWA Hygiene GmbH"; // 
				case 0xFDB5: return "ECSG"; // 
				case 0xFDB4: return "HP Inc"; // 
				case 0xFDB3: return "Audiodo AB"; // 
				case 0xFDB2: return "Portable Multimedia Ltd"; // 
				case 0xFDB1: return "Oura Health Ltd"; // 
				case 0xFDB0: return "Oura Health Ltd"; // 
				case 0xFDAF: return "Wiliot LTD"; // 
				case 0xFDAE: return "Houwa System Design, k.k."; // 
				case 0xFDAD: return "Houwa System Design, k.k."; // 
				case 0xFDAC: return "Tentacle Sync GmbH"; // 
				case 0xFDAB: return "Xiaomi Inc."; // 
				case 0xFDAA: return "Xiaomi Inc."; // 
				case 0xFDA9: return "Rhombus Systems, Inc."; // 
				case 0xFDA8: return "PSA Peugeot Citroën"; // 
				case 0xFDA7: return "WWZN Information Technology Company Limited"; // 
				case 0xFDA6: return "WWZN Information Technology Company Limited"; // 
				case 0xFDA5: return "Neurostim OAB, Inc."; // 
				case 0xFDA4: return "Inseego Corp."; // 
				case 0xFDA3: return "Inseego Corp."; // 
				case 0xFDA2: return "Groove X, Inc"; // 
				case 0xFDA1: return "Groove X, Inc"; // 
				case 0xFDA0: return "Secugen Corporation"; // 
				case 0xFD9F: return "VitalTech Affiliates LLC"; // 
				case 0xFD9E: return "The Coca-Cola Company"; // 
				case 0xFD9D: return "Gastec Corporation"; // 
				case 0xFD9C: return "Huawei Technologies Co., Ltd."; // 
				case 0xFD9B: return "Huawei Technologies Co., Ltd."; // 
				case 0xFD9A: return "Huawei Technologies Co., Ltd."; // 
				case 0xFD99: return "ABB Oy"; // 
				case 0xFD98: return "Disney Worldwide Services, Inc."; // 
				case 0xFD97: return "June Life, Inc."; // 
				case 0xFD96: return "Google LLC"; // 
				case 0xFD95: return "Rigado"; // 
				case 0xFD94: return "Hewlett Packard Enterprise"; // 
				case 0xFD93: return "Bayerische Motoren Werke AG"; // 
				case 0xFD92: return "Qualcomm Technologies International, Ltd. (QTIL)"; // 
				case 0xFD91: return "Groove X, Inc."; // 
				case 0xFD90: return "Guangzhou SuperSound Information Technology Co.,Ltd"; // 
				case 0xFD8E: return "Motorola Solutions"; // 
				case 0xFD8D: return "quip NYC Inc."; // 
				case 0xFD8C: return "Google LLC"; // 
				case 0xFD8B: return "Jigowatts Inc."; // 
				case 0xFD8A: return "Signify Netherlands B.V."; // 
				case 0xFD89: return "Urbanminded LTD"; // 
				case 0xFD88: return "Urbanminded LTD"; // 
				case 0xFD87: return "Google LLC"; // 
				case 0xFD86: return "Abbott"; // 
				case 0xFD85: return "Husqvarna AB"; // 
				case 0xFD84: return "Tile, Inc."; // 
				case 0xFD83: return "iNFORM Technology GmbH"; // 
				case 0xFD82: return "Sony Corporation"; // 
				case 0xFD81: return "CANDY HOUSE, Inc."; // 
				case 0xFD80: return "Phindex Technologies, Inc"; // 
				case 0xFD7F: return "Husqvarna AB"; // 
				case 0xFD7E: return "Samsung Electronics Co., Ltd."; // 
				case 0xFD7D: return "Center for Advanced Research Wernher Von Braun"; // 
				case 0xFD7C: return "Toshiba Information Systems(Japan) Corporation"; // 
				case 0xFD7B: return "WYZE LABS, INC."; // 
				case 0xFD7A: return "Withings"; // 
				case 0xFD79: return "Withings"; // 
				case 0xFD78: return "Withings"; // 
				case 0xFD77: return "Withings"; // 
				case 0xFD76: return "Insulet Corporation"; // 
				case 0xFD75: return "Insulet Corporation"; // 
				case 0xFD74: return "BRControls Products BV"; // 
				case 0xFD73: return "BRControls Products BV"; // 
				case 0xFD72: return "Logitech International SA"; // 
				case 0xFD71: return "GN Hearing A/S"; // 
				case 0xFD70: return "GuangDong Oppo Mobile Telecommunications Corp., Ltd"; // 
				case 0xFD6F: return "Apple, Inc."; // 
				case 0xFD6E: return "Polidea sp. z o.o."; // 
				case 0xFD6D: return "Sigma Elektro GmbH"; // 
				case 0xFD6C: return "Samsung Electronics Co., Ltd."; // 
				case 0xFD6B: return "rapitag GmbH"; // 
				case 0xFD6A: return "Emerson"; // 
				case 0xFD69: return "Samsung Electronics Co., Ltd"; // 
				case 0xFD68: return "Ubique Innovation AG"; // 
				case 0xFD67: return "Montblanc Simplo GmbH"; // 
				case 0xFD66: return "Zebra Technologies Corporation"; // 
				case 0xFD65: return "Razer Inc."; // 
				case 0xFD64: return "INRIA"; // 
				case 0xFD63: return "Google LLC"; // 
				case 0xFD62: return "Google LLC"; // 
				case 0xFD61: return "Arendi AG"; // 
				case 0xFD60: return "Sercomm Corporation"; // 
				case 0xFD5F: return "Meta Platforms Technologies, LLC"; // 
				case 0xFD5E: return "Tapkey GmbH"; // 
				case 0xFD5D: return "maxon motor ltd."; // 
				case 0xFD5C: return "React Mobile"; // 
				case 0xFD5B: return "V2SOFT INC."; // 
				case 0xFD5A: return "Samsung Electronics Co., Ltd."; // 
				case 0xFD59: return "Samsung Electronics Co., Ltd."; // 
				case 0xFD58: return "Volvo Car Corporation"; // 
				case 0xFD57: return "Volvo Car Corporation"; // 
				case 0xFD56: return "Resmed Ltd"; // 
				case 0xFD55: return "Braveheart Wireless, Inc."; // 
				case 0xFD54: return "Qingdao Haier Technology Co., Ltd."; // 
				case 0xFD53: return "PCI Private Limited"; // 
				case 0xFD52: return "UTC Fire and Security"; // 
				case 0xFD51: return "UTC Fire and Security"; // 
				case 0xFD50: return "Hangzhou Tuya Information  Technology Co., Ltd"; // 
				case 0xFD4F: return "SONITOR TECHNOLOGIES AS"; // 
				case 0xFD4E: return "70mai Co.,Ltd."; // 
				case 0xFD4D: return "70mai Co.,Ltd."; // 
				case 0xFD4C: return "Adolf Wuerth GmbH & Co KG"; // 
				case 0xFD4B: return "Samsung Electronics Co., Ltd."; // 
				case 0xFD4A: return "Sigma Elektro GmbH"; // 
				case 0xFD49: return "Panasonic Corporation"; // 
				case 0xFD48: return "Geberit International AG"; // 
				case 0xFD47: return "Liberty Global Inc."; // 
				case 0xFD46: return "Lemco IKE"; // 
				case 0xFD45: return "GB Solution co.,Ltd"; // 
				case 0xFD44: return "Apple Inc."; // 
				case 0xFD43: return "Apple Inc."; // 
				case 0xFD42: return "Globe (Jiangsu) Co.,Ltd"; // 
				case 0xFD41: return "Amazon Lab126"; // 
				case 0xFD40: return "Beflex Inc."; // 
				case 0xFD3F: return "Cognosos, Inc"; // 
				case 0xFD3E: return "Pure Watercraft, inc."; // 
				case 0xFD3D: return "Woan Technology (Shenzhen) Co., Ltd."; // 
				case 0xFD3C: return "Redline Communications Inc."; // 
				case 0xFD3B: return "Verkada Inc."; // 
				case 0xFD3A: return "Verkada Inc."; // 
				case 0xFD39: return "PREDIKTAS"; // 
				case 0xFD38: return "Danfoss A/S"; // 
				case 0xFD37: return "TireCheck GmbH"; // 
				case 0xFD36: return "Google LLC"; // 
				case 0xFD35: return "Transsion Holdings Limited"; // 
				case 0xFD34: return "Aerosens LLC."; // 
				case 0xFD33: return "DashLogic, Inc."; // 
				case 0xFD32: return "Gemalto Holding BV"; // 
				case 0xFD31: return "LG Electronics Inc."; // 
				case 0xFD30: return "Sesam Solutions BV"; // 
				case 0xFD2F: return "Bitstrata Systems Inc."; // 
				case 0xFD2E: return "Bitstrata Systems Inc."; // 
				case 0xFD2D: return "Xiaomi Inc."; // 
				case 0xFD2C: return "The Access Technologies"; // 
				case 0xFD2B: return "The Access Technologies"; // 
				case 0xFD2A: return "Sony Corporation"; // 
				case 0xFD29: return "Asahi Kasei Corporation"; // 
				case 0xFD28: return "Julius Blum GmbH"; // 
				case 0xFD27: return "Integrated Illumination Systems, Inc."; // 
				case 0xFD26: return "Novo Nordisk A/S"; // 
				case 0xFD25: return "GD Midea Air-Conditioning Equipment Co., Ltd."; // 
				case 0xFD24: return "GD Midea Air-Conditioning Equipment Co., Ltd."; // 
				case 0xFD23: return "DOM Sicherheitstechnik GmbH & Co. KG"; // 
				case 0xFD22: return "Huawei Technologies Co., Ltd."; // 
				case 0xFD21: return "Huawei Technologies Co., Ltd."; // 
				case 0xFD20: return "GN Hearing A/S"; // 
				case 0xFD1F: return "3M"; // 
				case 0xFD1E: return "Plume Design Inc."; // 
				case 0xFD1D: return "Samsung Electronics Co., Ltd"; // 
				case 0xFD1C: return "Brady Worldwide Inc."; // 
				case 0xFD1B: return "Helios Sports, Inc."; // 
				case 0xFD1A: return "CSIRO"; // 
				case 0xFD19: return "Smith & Nephew Medical Limited"; // 
				case 0xFD18: return "LEGIC Identsystems AG"; // 
				case 0xFD17: return "LEGIC Identsystems AG"; // 
				case 0xFD16: return "Sensitech, Inc."; // 
				case 0xFD15: return "Panasonic Corporation"; // 
				case 0xFD14: return "BRG Sports, Inc."; // 
				case 0xFD13: return "BRG Sports, Inc."; // 
				case 0xFD12: return "AEON MOTOR CO.,LTD."; // 
				case 0xFD11: return "AEON MOTOR CO.,LTD."; // 
				case 0xFD10: return "AEON MOTOR CO.,LTD."; // 
				case 0xFD0F: return "AEON MOTOR CO.,LTD."; // 
				case 0xFD0E: return "HerdDogg, Inc"; // 
				case 0xFD0D: return "Blecon Ltd"; // 
				case 0xFD0C: return "OSM HK Limited"; // 
				case 0xFD0B: return "Luminostics, Inc."; // 
				case 0xFD0A: return "Luminostics, Inc."; // 
				case 0xFD09: return "Cousins and Sears LLC"; // 
				case 0xFD08: return "Bull Group Incorporated Company"; // 
				case 0xFD07: return "Swedlock AB"; // 
				case 0xFD06: return "RACE-AI LLC"; // 
				case 0xFD05: return "Qualcomm Technologies, Inc."; // 
				case 0xFD04: return "Shure Inc."; // 
				case 0xFD03: return "Quuppa Oy"; // 
				case 0xFD02: return "LEGO System A/S"; // 
				case 0xFD01: return "Sanvita Medical Corporation"; // 
				case 0xFD00: return "FUTEK Advanced Sensor Technology, Inc."; // 
				case 0xFCFF: return "701x"; // 
				case 0xFCFE: return "Sonova Consumer Hearing GmbH"; // 
				case 0xFCFD: return "Barrot Technology Co.,Ltd."; // 
				case 0xFCFC: return "Barrot Technology Co.,Ltd."; // 
				case 0xFCFB: return "Shenzhen Benwei Media Co., Ltd."; // 
				case 0xFCFA: return "Leupold & Stevens, Inc."; // 
				case 0xFCF9: return "Leupold & Stevens, Inc."; // 
				case 0xFCF8: return "Honor Device Co., Ltd."; // 
				case 0xFCF7: return "Honor Device Co., Ltd."; // 
				case 0xFCF6: return "The Linux Foundation"; // 
				case 0xFCF5: return "Trident Communication Technology, LLC"; // 
				case 0xFCF4: return "Allegion"; // 
				case 0xFCF3: return "Armatura LLC"; // 
				case 0xFCF2: return "Bitwards Oy"; // 
				case 0xFCF1: return "Google LLC"; // 
				case 0xFCF0: return "Security Enhancement Systems, LLC"; // 
				case 0xFCEF: return "Divesoft s.r.o."; // 
				case 0xFCEE: return "Velentium, LLC"; // 
				case 0xFCED: return "Workaround Gmbh"; // 
				case 0xFCEC: return "Griffwerk GmbH"; // 
				case 0xFCEB: return "Avi-On"; // 
				case 0xFCEA: return "Chess Wise B.V."; // 
				case 0xFCE9: return "MindRhythm, Inc."; // 
				case 0xFCE8: return "ITT Industries"; // 
				case 0xFCE7: return "TKH Security B.V."; // 
				case 0xFCE6: return "Guard RFID Solutions Inc."; // 
				case 0xFCE5: return "Samsara Networks, Inc"; // 
				case 0xFCE4: return "Samsara Networks, Inc"; // 
				case 0xFCE3: return "Smith & Nephew Medical Limited"; // 
				case 0xFCE2: return "Baracoda Daily Healthtech"; // 
				case 0xFCE1: return "Sony Group Corporation"; // 
				case 0xFCE0: return "Akciju sabiedriba 'SAF TEHNIKA'"; // 
				case 0xFCDF: return "NIO USA, Inc."; // 
				case 0xFCDE: return "ARCTOP, INC."; // 
				case 0xFCDD: return "Mobilaris AB"; // 
				case 0xFCDC: return "Amazon.com Services, LLC"; // 
				case 0xFCDB: return "aconno GmbH"; // 
				case 0xFCDA: return "Draeger"; // 
				case 0xFCD9: return "Huso, INC"; // 
				case 0xFCD8: return "Appex Factory S.L."; // 
				case 0xFCD7: return "PowerPal Pty Ltd"; // 
				case 0xFCD6: return "SWISSINNO SOLUTIONS AG"; // 
				case 0xFCD5: return "Nortek Security & Control"; // 
				case 0xFCD4: return "OMRON HEALTHCARE"; // 
				case 0xFCD3: return "Fisher & Paykel Healthcare"; // 
				case 0xFCD2: return "Allterco Robotics ltd"; // 
				case 0xFCD1: return "Shenzhen Benwei Media Co.,Ltd."; // 
				case 0xFCD0: return "Laerdal Medical AS"; // 
				case 0xFCCF: return "Google LLC"; // 
				case 0xFCCE: return "Luna Health, Inc."; // 
				case 0xFCCD: return "Marshall Group AB"; // 
				case 0xFCCB: return "TOTO LTD."; // 
				case 0xFCCA: return "Cosmed s.r.l."; // 
				case 0xFCC9: return "SkyHawke Technologies"; // 
				case 0xFCC8: return "Allthenticate, Inc."; // 
				case 0xFCC7: return "PB INC."; // 
				case 0xFCC6: return "Wiliot LTD."; // 
				case 0xFCC5: return "OMRON(DALIAN) CO,.LTD."; // 
				case 0xFCC4: return "OMRON(DALIAN) CO,.LTD."; // 
				case 0xFCC3: return "HP Inc."; // 
				case 0xFCC2: return "Qualcomm Technologies, Inc."; // 
				case 0xFCC1: return "TIMECODE SYSTEMS LIMITED"; // 
				case 0xFCC0: return "Xiaomi Inc."; // 
				case 0xFCBF: return "ASSA ABLOY Opening Solutions Sweden AB"; // 
				case 0xFCBE: return "Musen Connect, Inc."; // 
				case 0xFCBD: return "Toshiba Corporation"; // 
				case 0xFCBC: return "Drowsy Digital, Inc."; // 
				case 0xFCBB: return "SharkNinja Operating LLC"; // 
				case 0xFCBA: return "BlueID GmbH"; // 
				case 0xFCB9: return "Lumi United Technology Co., Ltd"; // 
				case 0xFCB8: return "Ribbiot, INC."; // 
				case 0xFCB7: return "T-Mobile USA"; // 
				case 0xFCB6: return "OMRON HEALTHCARE Co., Ltd."; // 
				case 0xFCB5: return "OMRON HEALTHCARE Co., Ltd."; // 
				case 0xFCB4: return "OMRON HEALTHCARE Co., Ltd."; // 
				case 0xFCB3: return "SWEEN"; // 
				case 0xFCB2: return "Apple Inc."; // 
				case 0xFCB1: return "Google LLC"; // 
				case 0xFCB0: return "Ford Motor Company"; // 
				case 0xFCAF: return "AltoBeam Inc."; // 
				case 0xFCAE: return "Imagine Marketing Limited"; // 
				case 0xFCAD: return "Beijing 99help Safety Technology Co., Ltd"; // 
				case 0xFCAC: return "IRISS INC."; // 
				case 0xFCAB: return "IRISS INC."; // 
				case 0xFCAA: return "Spintly, Inc."; // 
				case 0xFCA9: return "Medtronic Inc."; // 
				case 0xFCA8: return "Medtronic Inc."; // 
				case 0xFCA7: return "Hubble Network Inc."; // 
				case 0xFCA6: return "Hubble Network Inc."; // 
				case 0xFCA5: return "HAYWARD INDUSTRIES, INC."; // 
				case 0xFCA4: return "HP Inc."; // 
				case 0xFCA3: return "Gunnebo Aktiebolag"; // 
				case 0xFCA2: return "Meizu Technology Co., Ltd."; // 
				case 0xFCA1: return "PF SCHWEISSTECHNOLOGIE GMBH"; // 
				case 0xFCA0: return "Apple Inc."; // 
				case 0xFC9F: return "Delta Development Team, Inc"; // 
				case 0xFC9E: return "Dell Computer Corporation"; // 
				case 0xFC9D: return "Lenovo (Singapore) Pte Ltd."; // 
				case 0xFC9B: return "Merry Electronics (S) Pte Ltd"; // 
				case 0xFC9A: return "Koppli AB"; // 
				case 0xFC99: return "Badger Meter"; // 
				case 0xFC98: return "Ruuvi Innovations Ltd."; // 
				case 0xFC97: return "Japan Display Inc."; // 
				case 0xFC96: return "LEGO System A/S"; // 
				case 0xFC95: return "Hippo Camp Software Ltd."; // 
				case 0xFC94: return "Apple Inc."; // 
				case 0xFC93: return "Komatsu Ltd."; // 
				case 0xFC92: return "Furuno Electric Co., Ltd."; // 
				case 0xFC91: return "Samsung Electronics Co., Ltd."; // 
				case 0xFC90: return "Wiliot LTD."; // 
				case 0xFC8F: return "Bose Corporation"; // 
				case 0xFC8E: return "Blue Iris Labs, Inc."; // 
				case 0xFC8D: return "Caire Inc."; // 
				case 0xFC8C: return "VusionGroup"; // 
				case 0xFC8B: return "Kaspersky Lab Middle East FZ-LLC"; // 
				case 0xFC8A: return "Intel Corporation"; // 
				case 0xFC89: return "Intel Corporation"; // 
				case 0xFC88: return "CCC del Uruguay"; // 
				case 0xFC87: return "Samsara Networks, Inc"; // 
				case 0xFC86: return "Samsara Networks, Inc"; // 
				case 0xFC85: return "Zhejiang Huanfu Technology Co., LTD"; // 
				case 0xFC84: return "NINGBO FOTILE KITCHENWARE CO., LTD."; // 
				case 0xFC83: return "iHealth Labs, Inc."; // 
				case 0xFC82: return "Zwift, Inc."; // 
				case 0xFC81: return "Axon Enterprise, Inc."; // 
				case 0xFC80: return "TELE System Communications Pte. Ltd."; // 
				case 0xFC7F: return "Southco"; // 
				case 0xFC7E: return "Harman International"; // 
				case 0xFC7D: return "MML US, Inc"; // 
				case 0xFC7C: return "Motorola Mobility, LLC"; // 
				case 0xFC7B: return "Testo SE & Co. KGaA"; // 
				case 0xFC7A: return "Outshiny India Private Limited"; // 
				case 0xFC79: return "LG Electronics Inc."; // 
				case 0xFC78: return "DHL"; // 
				case 0xFC77: return "SING SUN TECHNOLOGY (INTERNATIONAL) LIMITED"; // 
				case 0xFC76: return "Weber-Stephen Products LLC"; // 
				case 0xFC75: return "Xiaomi Inc."; // 
				case 0xFC74: return "EMBEINT INC"; // 
				case 0xFC73: return "Google LLC"; // 
				case 0xFC72: return "iodyne, LLC"; // 
				case 0xFC71: return "Hive-Zox International SA"; // 
				case 0xFC70: return "MOTIVE TECHNOLOGIES, INC."; // 
				case 0xFC6F: return "NextSense, Inc."; // 
				case 0xFC6E: return "stryker"; // 
				case 0xFC6D: return "MOTIVE TECHNOLOGIES, INC."; // 
				case 0xFC6C: return "Powerstick.com"; // 
				case 0xFC6B: return "Sonos Inc"; // 
				case 0xFC6A: return "Sonos Inc"; // 
				case 0xFC69: return "Harman International"; // 
				case 0xFC68: return "RIGH, INC."; // 
				case 0xFC67: return "Guangdong Hengqin Xingtong Technology Co.,ltd."; // 
				case 0xFC66: return "Xiaomi Inc."; // 
				case 0xFC65: return "Robor Electronics B.V."; // 
				case 0xFC64: return "Volvo Technology AB"; // 
				case 0xFC63: return "Volvo Technology AB"; // 
				case 0xFC62: return "SPRiNTUS GmbH"; // 
				case 0xFC61: return "QIKCONNEX LLC"; // 
				case 0xFC60: return "Ohme Operations UK Limited"; // 
				case 0xFC5F: return "PI-CRYSTAL INC."; // 
				case 0xFC5E: return "KUBU SMART LIMITED"; // 
				case 0xFC5D: return "GP Acoustics International Limited"; // 
				case 0xFC5C: return "PLASTIC RESEARCH AND DEVELOPMENT CORPORATION"; // 
				case 0xFC5B: return "Time Location Systems AS"; // 
				case 0xFC5A: return "LAST LOCK INC."; // 
				case 0xFC59: return "Ant Group Co., Ltd."; // 
				case 0xFC58: return "Shenzhen Minew Technologies Co., Ltd."; // 
				case 0xFC57: return "Ambient Life Inc."; // 
				case 0xFC56: return "Google LLC"; // 
				case 0xFC55: return "BYD Company Limited"; // 
				case 0xFC54: return "Shenzhen Yinwang Intelligent Technologies Co., Ltd."; // 
				case 0xFC53: return "LEGIC Identsystems AG"; // 
				case 0xFC52: return "LG Electronics Inc."; // 
				case 0xFC51: return "Ant Group Co., Ltd."; // 
				case 0xFC50: return "Ant Group Co., Ltd."; // 
				case 0xFC4F: return "WaveRF, Corp."; // 
				case 0xFC4E: return "Lodestar Technology Inc."; // 
				case 0xFC4D: return "Lodestar Technology Inc."; // 
				case 0xFC4C: return "HP Inc."; // 
				case 0xFC4B: return "WinMagic Inc."; // 
				case 0xFC4A: return "Shenzhen Shokz Co.,Ltd."; // 
				case 0xFC49: return "Golioth, Inc."; // 
				case 0xFC48: return "Michelin"; // 
				case 0xFC47: return "Shanghai Ingeek Technology Co., Ltd."; // 
				case 0xFC46: return "Xiaomi"; // 
				case 0xFC45: return "Mitsubishi Electric Corporation"; // 
				case 0xFC44: return "Block, Inc."; // 
				case 0xFC43: return "Atmosic Technologies, Inc."; // 
				case 0xFC41: return "Yoto Limited"; // 
				case 0xFC3F: return "Milwaukee Electric Tools"; // 
				case 0xFC3E: return "Google LLC"; // 
				case 0xFC3D: return "Reelables, Inc."; // 
				case 0xFC3C: return "Eforthink Technology Co., Ltd."; // 
				case 0xFC3B: return "BONX INC."; // 
				case 0xFC3A: return "C.O.B.O. SpA"; // 
				case 0xFC39: return "Harman International"; // 
				case 0xFC38: return "SetPoint Medical"; // 
				case 0xFC37: return "TANlock GmbH"; // 
				case 0xFC36: return "PharmaSens AG"; // 
				case 0xFC35: return "Metabowerke GmbH"; // 
				case 0xFC34: return "Mitsubishi Electric Corporation"; // 
				case 0xFC32: return "InPlay, Inc."; // 
				case 0xFC31: return "Ford Motor Company"; // 
				case 0xFC30: return "Arashi Vision Inc."; // 
				case 0xFC2C: return "Verkada Inc."; // 
				case 0xFC2B: return "Elder Technologies, Inc"; // 
				case 0xFC2A: return "Troo Corporation"; // 
                // endupdatefile:

            }
            return null; // $"?{uuid:X2}";
        }

        private static Registration[] Registrations = new Registration[]
        {
            // updatefile:
            // url:https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/uuids/member_uuids.yaml
            // file:member_uuids.yaml
            // template:new Registration(HEX, "NAME", "DATEMMDDYYYY"),
            // startupdatefile:
				case 0xFEFF: return "GN Netcom"; // 
				case 0xFEFE: return "GN Hearing A/S"; // 
				case 0xFEFD: return "Gimbal, Inc."; // 
				case 0xFEFC: return "Gimbal, Inc."; // 
				case 0xFEFB: return "Telit Wireless Solutions (Formerly Stollmann E+V GmbH)"; // 
				case 0xFEFA: return "PayPal, Inc."; // 
				case 0xFEF9: return "PayPal, Inc."; // 
				case 0xFEF8: return "Aplix Corporation"; // 
				case 0xFEF7: return "Aplix Corporation"; // 
				case 0xFEF6: return "Wicentric, Inc."; // 
				case 0xFEF5: return "Dialog Semiconductor GmbH"; // 
				case 0xFEF4: return "Google LLC"; // 
				case 0xFEF3: return "Google LLC"; // 
				case 0xFEF2: return "CSR"; // 
				case 0xFEF1: return "CSR"; // 
				case 0xFEF0: return "Intel"; // 
				case 0xFEEF: return "Polar Electro Oy"; // 
				case 0xFEEE: return "Polar Electro Oy"; // 
				case 0xFEED: return "Tile, Inc."; // 
				case 0xFEEC: return "Tile, Inc."; // 
				case 0xFEEB: return "Swirl Networks, Inc."; // 
				case 0xFEEA: return "Swirl Networks, Inc."; // 
				case 0xFEE9: return "Quintic Corp."; // 
				case 0xFEE8: return "Quintic Corp."; // 
				case 0xFEE7: return "Tencent Holdings Limited."; // 
				case 0xFEE6: return "Silvair, Inc."; // 
				case 0xFEE5: return "Nordic Semiconductor ASA"; // 
				case 0xFEE4: return "Nordic Semiconductor ASA"; // 
				case 0xFEE3: return "Anki, Inc."; // 
				case 0xFEE2: return "Anki, Inc."; // 
				case 0xFEE1: return "Anhui Huami Information Technology Co., Ltd."; // 
				case 0xFEE0: return "Anhui Huami Information Technology Co., Ltd."; // 
				case 0xFEDE: return "Coin, Inc."; // 
				case 0xFEDD: return "Jawbone"; // 
				case 0xFEDC: return "Jawbone"; // 
				case 0xFEDB: return "Perka, Inc."; // 
				case 0xFEDA: return "ISSC Technologies Corp."; // 
				case 0xFED9: return "Pebble Technology Corporation"; // 
				case 0xFED8: return "Google LLC"; // 
				case 0xFED7: return "Broadcom"; // 
				case 0xFED6: return "Broadcom"; // 
				case 0xFED5: return "Plantronics Inc."; // 
				case 0xFED4: return "Apple, Inc."; // 
				case 0xFED3: return "Apple, Inc."; // 
				case 0xFED2: return "Apple, Inc."; // 
				case 0xFED1: return "Apple, Inc."; // 
				case 0xFED0: return "Apple, Inc."; // 
				case 0xFECF: return "Apple, Inc."; // 
				case 0xFECE: return "Apple, Inc."; // 
				case 0xFECD: return "Apple, Inc."; // 
				case 0xFECC: return "Apple, Inc."; // 
				case 0xFECB: return "Apple, Inc."; // 
				case 0xFECA: return "Apple, Inc."; // 
				case 0xFEC9: return "Apple, Inc."; // 
				case 0xFEC8: return "Apple, Inc."; // 
				case 0xFEC7: return "Apple, Inc."; // 
				case 0xFEC6: return "Kocomojo, LLC"; // 
				case 0xFEC5: return "Realtek Semiconductor Corp."; // 
				case 0xFEC4: return "PLUS Location Systems"; // 
				case 0xFEC3: return "360fly, Inc."; // 
				case 0xFEC2: return "Blue Spark Technologies, Inc."; // 
				case 0xFEC1: return "KDDI Corporation"; // 
				case 0xFEC0: return "KDDI Corporation"; // 
				case 0xFEBF: return "Nod, Inc."; // 
				case 0xFEBE: return "Bose Corporation"; // 
				case 0xFEBD: return "Clover Network, Inc"; // 
				case 0xFEBC: return "Dexcom Inc"; // 
				case 0xFEBB: return "adafruit industries"; // 
				case 0xFEBA: return "Tencent Holdings Limited"; // 
				case 0xFEB9: return "LG Electronics"; // 
				case 0xFEB8: return "Meta Platforms, Inc."; // 
				case 0xFEB7: return "Meta Platforms, Inc."; // 
				case 0xFEB6: return "Vencer Co., Ltd"; // 
				case 0xFEB5: return "WiSilica Inc."; // 
				case 0xFEB4: return "WiSilica Inc."; // 
				case 0xFEB3: return "Taobao"; // 
				case 0xFEB2: return "Microsoft Corporation"; // 
				case 0xFEB1: return "Electronics Tomorrow Limited"; // 
				case 0xFEB0: return "Nest Labs Inc"; // 
				case 0xFEAF: return "Nest Labs Inc"; // 
				case 0xFEAE: return "Nokia"; // 
				case 0xFEAD: return "Nokia"; // 
				case 0xFEAC: return "Nokia"; // 
				case 0xFEAB: return "Nokia"; // 
				case 0xFEAA: return "Google LLC"; // 
				case 0xFEA9: return "Savant Systems LLC"; // 
				case 0xFEA8: return "Savant Systems LLC"; // 
				case 0xFEA7: return "UTC Fire and Security"; // 
				case 0xFEA6: return "GoPro, Inc."; // 
				case 0xFEA5: return "GoPro, Inc."; // 
				case 0xFEA4: return "Paxton Access Ltd"; // 
				case 0xFEA3: return "ITT Industries"; // 
				case 0xFEA0: return "Google LLC"; // 
				case 0xFE9F: return "Google LLC"; // 
				case 0xFE9E: return "Renesas Design Netherlands B.V."; // 
				case 0xFE9D: return "Mobiquity Networks Inc"; // 
				case 0xFE9C: return "GSI Laboratories, Inc."; // 
				case 0xFE9B: return "Samsara Networks, Inc"; // 
				case 0xFE9A: return "Estimote"; // 
				case 0xFE99: return "Currant Inc"; // 
				case 0xFE98: return "Currant Inc"; // 
				case 0xFE97: return "Tesla Motors Inc."; // 
				case 0xFE96: return "Tesla Motors Inc."; // 
				case 0xFE95: return "Xiaomi Inc."; // 
				case 0xFE94: return "OttoQ In"; // 
				case 0xFE93: return "OttoQ In"; // 
				case 0xFE92: return "Jarden Safety & Security"; // 
				case 0xFE91: return "Shanghai Imilab Technology Co.,Ltd"; // 
				case 0xFE90: return "JUMA"; // 
				case 0xFE8F: return "CSR"; // 
				case 0xFE8E: return "ARM Ltd"; // 
				case 0xFE8D: return "Interaxon Inc."; // 
				case 0xFE8C: return "TRON Forum"; // 
				case 0xFE8B: return "Apple, Inc."; // 
				case 0xFE8A: return "Apple, Inc."; // 
				case 0xFE89: return "B&O Play A/S"; // 
				case 0xFE88: return "SALTO SYSTEMS S.L."; // 
				case 0xFE87: return "Qingdao Yeelink Information Technology Co., Ltd. ( 青岛亿联客信息技术有限公司 )"; // 
				case 0xFE86: return "HUAWEI Technologies Co., Ltd"; // 
				case 0xFE85: return "RF Digital Corp"; // 
				case 0xFE84: return "RF Digital Corp"; // 
				case 0xFE83: return "Blue Bite"; // 
				case 0xFE82: return "Medtronic Inc."; // 
				case 0xFE81: return "Medtronic Inc."; // 
				case 0xFE80: return "Doppler Lab"; // 
				case 0xFE7F: return "Doppler Lab"; // 
				case 0xFE7E: return "Awear Solutions Ltd"; // 
				case 0xFE7D: return "Aterica Health Inc."; // 
				case 0xFE7C: return "Telit Wireless Solutions (Formerly Stollmann E+V GmbH)"; // 
				case 0xFE7B: return "Orion Labs, Inc."; // 
				case 0xFE7A: return "Bragi GmbH"; // 
				case 0xFE79: return "Zebra Technologies"; // 
				case 0xFE78: return "Hewlett-Packard Company"; // 
				case 0xFE77: return "Hewlett-Packard Company"; // 
				case 0xFE76: return "TangoMe"; // 
				case 0xFE75: return "TangoMe"; // 
				case 0xFE74: return "unwire"; // 
				case 0xFE73: return "Abbott (formerly St. Jude Medical, Inc.)"; // 
				case 0xFE72: return "Abbott (formerly St. Jude Medical, Inc.)"; // 
				case 0xFE71: return "Plume Design Inc"; // 
				case 0xFE70: return "Beijing Jingdong Century Trading Co., Ltd."; // 
				case 0xFE6F: return "LINE Corporation"; // 
				case 0xFE6E: return "The University of Tokyo"; // 
				case 0xFE6D: return "The University of Tokyo"; // 
				case 0xFE6C: return "TASER International, Inc."; // 
				case 0xFE6B: return "TASER International, Inc."; // 
				case 0xFE6A: return "Kontakt Micro-Location Sp. z o.o."; // 
				case 0xFE69: return "Capsle Technologies Inc."; // 
				case 0xFE68: return "Capsle Technologies Inc."; // 
				case 0xFE67: return "Lab Sensor Solutions"; // 
				case 0xFE66: return "Intel Corporation"; // 
				case 0xFE65: return "CHIPOLO d.o.o."; // 
				case 0xFE64: return "Siemens AG"; // 
				case 0xFE63: return "Connected Yard, Inc."; // 
				case 0xFE62: return "Indagem Tech LLC"; // 
				case 0xFE61: return "Logitech International SA"; // 
				case 0xFE60: return "Lierda Science & Technology Group Co., Ltd."; // 
				case 0xFE5F: return "Eyefi, Inc."; // 
				case 0xFE5E: return "Plastc Corporation"; // 
				case 0xFE5D: return "Grundfos A/S"; // 
				case 0xFE5C: return "million hunters GmbH"; // 
				case 0xFE5B: return "GT-tronics HK Ltd"; // 
				case 0xFE5A: return "Cronologics Corporation"; // 
				case 0xFE59: return "Nordic Semiconductor ASA"; // 
				case 0xFE58: return "Nordic Semiconductor ASA"; // 
				case 0xFE57: return "Dotted Labs"; // 
				case 0xFE56: return "Google LLC"; // 
				case 0xFE55: return "Google LLC"; // 
				case 0xFE54: return "Motiv, Inc."; // 
				case 0xFE53: return "3M"; // 
				case 0xFE52: return "SetPoint Medical"; // 
				case 0xFE51: return "SRAM"; // 
				case 0xFE50: return "Google LLC"; // 
				case 0xFE4F: return "Molekule, Inc."; // 
				case 0xFE4E: return "NTT docomo"; // 
				case 0xFE4D: return "Casambi Technologies Oy"; // 
				case 0xFE4C: return "Volkswagen AG"; // 
				case 0xFE4B: return "Signify Netherlands B.V. (formerly Philips Lighting B.V.)"; // 
				case 0xFE4A: return "OMRON HEALTHCARE Co., Ltd."; // 
				case 0xFE49: return "SenionLab AB"; // 
				case 0xFE48: return "General Motors"; // 
				case 0xFE47: return "General Motors"; // 
				case 0xFE46: return "B&O Play A/S"; // 
				case 0xFE45: return "Snapchat Inc"; // 
				case 0xFE44: return "SK Telecom"; // 
				case 0xFE43: return "Andreas Stihl AG & Co. KG"; // 
				case 0xFE42: return "Nets A/S"; // 
				case 0xFE41: return "Inugo Systems Limited"; // 
				case 0xFE40: return "Inugo Systems Limited"; // 
				case 0xFE3F: return "Friday Labs Limited"; // 
				case 0xFE3E: return "BD Medical"; // 
				case 0xFE3D: return "BD Medical"; // 
				case 0xFE3C: return "alibaba"; // 
				case 0xFE3B: return "Dolby Laboratories"; // 
				case 0xFE3A: return "TTS Tooltechnic Systems AG & Co. KG"; // 
				case 0xFE39: return "TTS Tooltechnic Systems AG & Co. KG"; // 
				case 0xFE36: return "HUAWEI Technologies Co., Ltd"; // 
				case 0xFE35: return "HUAWEI Technologies Co., Ltd"; // 
				case 0xFE34: return "SmallLoop LLC"; // 
				case 0xFE33: return "CHIPOLO d.o.o."; // 
				case 0xFE32: return "Pro-Mark, Inc."; // 
				case 0xFE31: return "Volkswagen AG"; // 
				case 0xFE30: return "Volkswagen AG"; // 
				case 0xFE2F: return "CRESCO Wireless, Inc"; // 
				case 0xFE2E: return "ERi,Inc."; // 
				case 0xFE2D: return "LAMPLIGHT Co., Ltd."; // 
				case 0xFE2C: return "Google LLC"; // 
				case 0xFE2B: return "ITT Industries"; // 
				case 0xFE2A: return "DaisyWorks, Inc."; // 
				case 0xFE29: return "Gibson Innovations"; // 
				case 0xFE28: return "Ayla Networks"; // 
				case 0xFE27: return "Google LLC"; // 
				case 0xFE26: return "Google LLC"; // 
				case 0xFE25: return "Apple, Inc."; // 
				case 0xFE24: return "August Home Inc"; // 
				case 0xFE23: return "Zoll Medical Corporation"; // 
				case 0xFE22: return "Zoll Medical Corporation"; // 
				case 0xFE21: return "Bose Corporation"; // 
				case 0xFE20: return "Emerson"; // 
				case 0xFE1F: return "Garmin International, Inc."; // 
				case 0xFE1E: return "LAMPLIGHT Co., Ltd."; // 
				case 0xFE1D: return "Illuminati Instrument Corporation"; // 
				case 0xFE1C: return "NetMedia, Inc."; // 
				case 0xFE1B: return "Tyto Life LLC"; // 
				case 0xFE1A: return "Tyto Life LLC"; // 
				case 0xFE19: return "Google LLC"; // 
				case 0xFE18: return "Runtime, Inc."; // 
				case 0xFE17: return "Telit Wireless Solutions GmbH"; // 
				case 0xFE16: return "Footmarks, Inc."; // 
				case 0xFE15: return "Amazon.com Services, Inc.."; // 
				case 0xFE14: return "Flextronics International USA Inc."; // 
				case 0xFE13: return "Apple Inc."; // 
				case 0xFE12: return "M-Way Solutions GmbH"; // 
				case 0xFE11: return "GMC-I Messtechnik GmbH"; // 
				case 0xFE10: return "LAPIS Technology Co., Ltd."; // 
				case 0xFE0F: return "Signify Netherlands B.V. (formerly Philips Lighting B.V.)"; // 
				case 0xFE0E: return "Setec Pty Ltd"; // 
				case 0xFE0D: return "Procter & Gamble"; // 
				case 0xFE0C: return "Procter & Gamble"; // 
				case 0xFE0B: return "ruwido austria gmbh"; // 
				case 0xFE0A: return "ruwido austria gmbh"; // 
				case 0xFE09: return "Pillsy, Inc."; // 
				case 0xFE08: return "Microsoft"; // 
				case 0xFE07: return "Sonos, Inc."; // 
				case 0xFE06: return "Qualcomm Technologies, Inc."; // 
				case 0xFE05: return "CORE Transport Technologies NZ Limited"; // 
				case 0xFE04: return "Motorola Solutions, Inc."; // 
				case 0xFE03: return "Amazon.com Services, Inc."; // 
				case 0xFE02: return "Robert Bosch GmbH"; // 
				case 0xFE01: return "Duracell U.S. Operations Inc."; // 
				case 0xFE00: return "Amazon.com Services, Inc."; // 
				case 0xFDFF: return "OSRAM GmbH"; // 
				case 0xFDFE: return "ADHERIUM(NZ) LIMITED"; // 
				case 0xFDFD: return "RecursiveSoft Inc."; // 
				case 0xFDFC: return "Optrel AG"; // 
				case 0xFDFB: return "Tandem Diabetes Care"; // 
				case 0xFDFA: return "Tandem Diabetes Care"; // 
				case 0xFDF9: return "INIA"; // 
				case 0xFDF8: return "Onvocal"; // 
				case 0xFDF7: return "HP Inc."; // 
				case 0xFDF6: return "AIAIAI ApS"; // 
				case 0xFDF5: return "Milwaukee Electric Tools"; // 
				case 0xFDF4: return "O. E. M. Controls, Inc."; // 
				case 0xFDF3: return "Amersports"; // 
				case 0xFDF2: return "AMICCOM Electronics Corporation"; // 
				case 0xFDF1: return "LAMPLIGHT Co.,Ltd"; // 
				case 0xFDF0: return "Google LLC"; // 
				case 0xFDEF: return "ART AND PROGRAM, INC."; // 
				case 0xFDEE: return "Huawei Technologies Co., Ltd."; // 
				case 0xFDED: return "Pole Star"; // 
				case 0xFDEC: return "Mannkind Corporation"; // 
				case 0xFDEB: return "Syntronix Corporation"; // 
				case 0xFDEA: return "SeeScan, Inc"; // 
				case 0xFDE9: return "Spacesaver Corporation"; // 
				case 0xFDE8: return "Robert Bosch GmbH"; // 
				case 0xFDE7: return "SECOM Co., LTD"; // 
				case 0xFDE6: return "Intelletto Technologies Inc"; // 
				case 0xFDE5: return "SMK Corporation"; // 
				case 0xFDE4: return "JUUL Labs, Inc."; // 
				case 0xFDE3: return "Abbott Diabetes Care"; // 
				case 0xFDE2: return "Google LLC"; // 
				case 0xFDE1: return "Fortin Electronic Systems"; // 
				case 0xFDE0: return "John Deere"; // 
				case 0xFDDF: return "Harman International"; // 
				case 0xFDDE: return "Noodle Technology Inc."; // 
				case 0xFDDD: return "Arch Systems Inc"; // 
				case 0xFDDC: return "4iiii Innovations Inc."; // 
				case 0xFDDB: return "Samsung Electronics Co., Ltd."; // 
				case 0xFDDA: return "MHCS"; // 
				case 0xFDD9: return "Jiangsu Teranovo Tech Co., Ltd."; // 
				case 0xFDD8: return "Jiangsu Teranovo Tech Co., Ltd."; // 
				case 0xFDD7: return "Copeland Cold Chain LP"; // 
				case 0xFDD6: return "Ministry of Supply"; // 
				case 0xFDD5: return "Brompton Bicycle Ltd"; // 
				case 0xFDD4: return "LX Solutions Pty Limited"; // 
				case 0xFDD3: return "FUBA Automotive Electronics GmbH"; // 
				case 0xFDD2: return "Bose Corporation"; // 
				case 0xFDD1: return "Huawei Technologies Co., Ltd"; // 
				case 0xFDD0: return "Huawei Technologies Co., Ltd"; // 
				case 0xFDCF: return "Nalu Medical, Inc"; // 
				case 0xFDCE: return "SENNHEISER electronic GmbH & Co. KG"; // 
				case 0xFDCD: return "Qingping Technology (Beijing) Co., Ltd."; // 
				case 0xFDCC: return "Shoof Technologies"; // 
				case 0xFDCB: return "Meggitt SA"; // 
				case 0xFDCA: return "Fortin Electronic Systems"; // 
				case 0xFDC9: return "Busch-Jaeger Elektro GmbH"; // 
				case 0xFDC8: return "Hach – Danaher"; // 
				case 0xFDC7: return "Eli Lilly and Company"; // 
				case 0xFDC6: return "Eli Lilly and Company"; // 
				case 0xFDC5: return "Automatic Labs"; // 
				case 0xFDC4: return "Simavita (Aust) Pty Ltd"; // 
				case 0xFDC3: return "Baidu Online Network Technology (Beijing) Co., Ltd"; // 
				case 0xFDC2: return "Baidu Online Network Technology (Beijing) Co., Ltd"; // 
				case 0xFDC1: return "Hunter Douglas"; // 
				case 0xFDC0: return "Hunter Douglas"; // 
				case 0xFDBF: return "California Things Inc."; // 
				case 0xFDBE: return "California Things Inc."; // 
				case 0xFDBD: return "Clover Network, Inc."; // 
				case 0xFDBC: return "Emerson"; // 
				case 0xFDBB: return "Profoto"; // 
				case 0xFDB8: return "LivaNova USA Inc."; // 
				case 0xFDB7: return "LivaNova USA Inc."; // 
				case 0xFDB6: return "GWA Hygiene GmbH"; // 
				case 0xFDB5: return "ECSG"; // 
				case 0xFDB4: return "HP Inc"; // 
				case 0xFDB3: return "Audiodo AB"; // 
				case 0xFDB2: return "Portable Multimedia Ltd"; // 
				case 0xFDB1: return "Oura Health Ltd"; // 
				case 0xFDB0: return "Oura Health Ltd"; // 
				case 0xFDAF: return "Wiliot LTD"; // 
				case 0xFDAE: return "Houwa System Design, k.k."; // 
				case 0xFDAD: return "Houwa System Design, k.k."; // 
				case 0xFDAC: return "Tentacle Sync GmbH"; // 
				case 0xFDAB: return "Xiaomi Inc."; // 
				case 0xFDAA: return "Xiaomi Inc."; // 
				case 0xFDA9: return "Rhombus Systems, Inc."; // 
				case 0xFDA8: return "PSA Peugeot Citroën"; // 
				case 0xFDA7: return "WWZN Information Technology Company Limited"; // 
				case 0xFDA6: return "WWZN Information Technology Company Limited"; // 
				case 0xFDA5: return "Neurostim OAB, Inc."; // 
				case 0xFDA4: return "Inseego Corp."; // 
				case 0xFDA3: return "Inseego Corp."; // 
				case 0xFDA2: return "Groove X, Inc"; // 
				case 0xFDA1: return "Groove X, Inc"; // 
				case 0xFDA0: return "Secugen Corporation"; // 
				case 0xFD9F: return "VitalTech Affiliates LLC"; // 
				case 0xFD9E: return "The Coca-Cola Company"; // 
				case 0xFD9D: return "Gastec Corporation"; // 
				case 0xFD9C: return "Huawei Technologies Co., Ltd."; // 
				case 0xFD9B: return "Huawei Technologies Co., Ltd."; // 
				case 0xFD9A: return "Huawei Technologies Co., Ltd."; // 
				case 0xFD99: return "ABB Oy"; // 
				case 0xFD98: return "Disney Worldwide Services, Inc."; // 
				case 0xFD97: return "June Life, Inc."; // 
				case 0xFD96: return "Google LLC"; // 
				case 0xFD95: return "Rigado"; // 
				case 0xFD94: return "Hewlett Packard Enterprise"; // 
				case 0xFD93: return "Bayerische Motoren Werke AG"; // 
				case 0xFD92: return "Qualcomm Technologies International, Ltd. (QTIL)"; // 
				case 0xFD91: return "Groove X, Inc."; // 
				case 0xFD90: return "Guangzhou SuperSound Information Technology Co.,Ltd"; // 
				case 0xFD8E: return "Motorola Solutions"; // 
				case 0xFD8D: return "quip NYC Inc."; // 
				case 0xFD8C: return "Google LLC"; // 
				case 0xFD8B: return "Jigowatts Inc."; // 
				case 0xFD8A: return "Signify Netherlands B.V."; // 
				case 0xFD89: return "Urbanminded LTD"; // 
				case 0xFD88: return "Urbanminded LTD"; // 
				case 0xFD87: return "Google LLC"; // 
				case 0xFD86: return "Abbott"; // 
				case 0xFD85: return "Husqvarna AB"; // 
				case 0xFD84: return "Tile, Inc."; // 
				case 0xFD83: return "iNFORM Technology GmbH"; // 
				case 0xFD82: return "Sony Corporation"; // 
				case 0xFD81: return "CANDY HOUSE, Inc."; // 
				case 0xFD80: return "Phindex Technologies, Inc"; // 
				case 0xFD7F: return "Husqvarna AB"; // 
				case 0xFD7E: return "Samsung Electronics Co., Ltd."; // 
				case 0xFD7D: return "Center for Advanced Research Wernher Von Braun"; // 
				case 0xFD7C: return "Toshiba Information Systems(Japan) Corporation"; // 
				case 0xFD7B: return "WYZE LABS, INC."; // 
				case 0xFD7A: return "Withings"; // 
				case 0xFD79: return "Withings"; // 
				case 0xFD78: return "Withings"; // 
				case 0xFD77: return "Withings"; // 
				case 0xFD76: return "Insulet Corporation"; // 
				case 0xFD75: return "Insulet Corporation"; // 
				case 0xFD74: return "BRControls Products BV"; // 
				case 0xFD73: return "BRControls Products BV"; // 
				case 0xFD72: return "Logitech International SA"; // 
				case 0xFD71: return "GN Hearing A/S"; // 
				case 0xFD70: return "GuangDong Oppo Mobile Telecommunications Corp., Ltd"; // 
				case 0xFD6F: return "Apple, Inc."; // 
				case 0xFD6E: return "Polidea sp. z o.o."; // 
				case 0xFD6D: return "Sigma Elektro GmbH"; // 
				case 0xFD6C: return "Samsung Electronics Co., Ltd."; // 
				case 0xFD6B: return "rapitag GmbH"; // 
				case 0xFD6A: return "Emerson"; // 
				case 0xFD69: return "Samsung Electronics Co., Ltd"; // 
				case 0xFD68: return "Ubique Innovation AG"; // 
				case 0xFD67: return "Montblanc Simplo GmbH"; // 
				case 0xFD66: return "Zebra Technologies Corporation"; // 
				case 0xFD65: return "Razer Inc."; // 
				case 0xFD64: return "INRIA"; // 
				case 0xFD63: return "Google LLC"; // 
				case 0xFD62: return "Google LLC"; // 
				case 0xFD61: return "Arendi AG"; // 
				case 0xFD60: return "Sercomm Corporation"; // 
				case 0xFD5F: return "Meta Platforms Technologies, LLC"; // 
				case 0xFD5E: return "Tapkey GmbH"; // 
				case 0xFD5D: return "maxon motor ltd."; // 
				case 0xFD5C: return "React Mobile"; // 
				case 0xFD5B: return "V2SOFT INC."; // 
				case 0xFD5A: return "Samsung Electronics Co., Ltd."; // 
				case 0xFD59: return "Samsung Electronics Co., Ltd."; // 
				case 0xFD58: return "Volvo Car Corporation"; // 
				case 0xFD57: return "Volvo Car Corporation"; // 
				case 0xFD56: return "Resmed Ltd"; // 
				case 0xFD55: return "Braveheart Wireless, Inc."; // 
				case 0xFD54: return "Qingdao Haier Technology Co., Ltd."; // 
				case 0xFD53: return "PCI Private Limited"; // 
				case 0xFD52: return "UTC Fire and Security"; // 
				case 0xFD51: return "UTC Fire and Security"; // 
				case 0xFD50: return "Hangzhou Tuya Information  Technology Co., Ltd"; // 
				case 0xFD4F: return "SONITOR TECHNOLOGIES AS"; // 
				case 0xFD4E: return "70mai Co.,Ltd."; // 
				case 0xFD4D: return "70mai Co.,Ltd."; // 
				case 0xFD4C: return "Adolf Wuerth GmbH & Co KG"; // 
				case 0xFD4B: return "Samsung Electronics Co., Ltd."; // 
				case 0xFD4A: return "Sigma Elektro GmbH"; // 
				case 0xFD49: return "Panasonic Corporation"; // 
				case 0xFD48: return "Geberit International AG"; // 
				case 0xFD47: return "Liberty Global Inc."; // 
				case 0xFD46: return "Lemco IKE"; // 
				case 0xFD45: return "GB Solution co.,Ltd"; // 
				case 0xFD44: return "Apple Inc."; // 
				case 0xFD43: return "Apple Inc."; // 
				case 0xFD42: return "Globe (Jiangsu) Co.,Ltd"; // 
				case 0xFD41: return "Amazon Lab126"; // 
				case 0xFD40: return "Beflex Inc."; // 
				case 0xFD3F: return "Cognosos, Inc"; // 
				case 0xFD3E: return "Pure Watercraft, inc."; // 
				case 0xFD3D: return "Woan Technology (Shenzhen) Co., Ltd."; // 
				case 0xFD3C: return "Redline Communications Inc."; // 
				case 0xFD3B: return "Verkada Inc."; // 
				case 0xFD3A: return "Verkada Inc."; // 
				case 0xFD39: return "PREDIKTAS"; // 
				case 0xFD38: return "Danfoss A/S"; // 
				case 0xFD37: return "TireCheck GmbH"; // 
				case 0xFD36: return "Google LLC"; // 
				case 0xFD35: return "Transsion Holdings Limited"; // 
				case 0xFD34: return "Aerosens LLC."; // 
				case 0xFD33: return "DashLogic, Inc."; // 
				case 0xFD32: return "Gemalto Holding BV"; // 
				case 0xFD31: return "LG Electronics Inc."; // 
				case 0xFD30: return "Sesam Solutions BV"; // 
				case 0xFD2F: return "Bitstrata Systems Inc."; // 
				case 0xFD2E: return "Bitstrata Systems Inc."; // 
				case 0xFD2D: return "Xiaomi Inc."; // 
				case 0xFD2C: return "The Access Technologies"; // 
				case 0xFD2B: return "The Access Technologies"; // 
				case 0xFD2A: return "Sony Corporation"; // 
				case 0xFD29: return "Asahi Kasei Corporation"; // 
				case 0xFD28: return "Julius Blum GmbH"; // 
				case 0xFD27: return "Integrated Illumination Systems, Inc."; // 
				case 0xFD26: return "Novo Nordisk A/S"; // 
				case 0xFD25: return "GD Midea Air-Conditioning Equipment Co., Ltd."; // 
				case 0xFD24: return "GD Midea Air-Conditioning Equipment Co., Ltd."; // 
				case 0xFD23: return "DOM Sicherheitstechnik GmbH & Co. KG"; // 
				case 0xFD22: return "Huawei Technologies Co., Ltd."; // 
				case 0xFD21: return "Huawei Technologies Co., Ltd."; // 
				case 0xFD20: return "GN Hearing A/S"; // 
				case 0xFD1F: return "3M"; // 
				case 0xFD1E: return "Plume Design Inc."; // 
				case 0xFD1D: return "Samsung Electronics Co., Ltd"; // 
				case 0xFD1C: return "Brady Worldwide Inc."; // 
				case 0xFD1B: return "Helios Sports, Inc."; // 
				case 0xFD1A: return "CSIRO"; // 
				case 0xFD19: return "Smith & Nephew Medical Limited"; // 
				case 0xFD18: return "LEGIC Identsystems AG"; // 
				case 0xFD17: return "LEGIC Identsystems AG"; // 
				case 0xFD16: return "Sensitech, Inc."; // 
				case 0xFD15: return "Panasonic Corporation"; // 
				case 0xFD14: return "BRG Sports, Inc."; // 
				case 0xFD13: return "BRG Sports, Inc."; // 
				case 0xFD12: return "AEON MOTOR CO.,LTD."; // 
				case 0xFD11: return "AEON MOTOR CO.,LTD."; // 
				case 0xFD10: return "AEON MOTOR CO.,LTD."; // 
				case 0xFD0F: return "AEON MOTOR CO.,LTD."; // 
				case 0xFD0E: return "HerdDogg, Inc"; // 
				case 0xFD0D: return "Blecon Ltd"; // 
				case 0xFD0C: return "OSM HK Limited"; // 
				case 0xFD0B: return "Luminostics, Inc."; // 
				case 0xFD0A: return "Luminostics, Inc."; // 
				case 0xFD09: return "Cousins and Sears LLC"; // 
				case 0xFD08: return "Bull Group Incorporated Company"; // 
				case 0xFD07: return "Swedlock AB"; // 
				case 0xFD06: return "RACE-AI LLC"; // 
				case 0xFD05: return "Qualcomm Technologies, Inc."; // 
				case 0xFD04: return "Shure Inc."; // 
				case 0xFD03: return "Quuppa Oy"; // 
				case 0xFD02: return "LEGO System A/S"; // 
				case 0xFD01: return "Sanvita Medical Corporation"; // 
				case 0xFD00: return "FUTEK Advanced Sensor Technology, Inc."; // 
				case 0xFCFF: return "701x"; // 
				case 0xFCFE: return "Sonova Consumer Hearing GmbH"; // 
				case 0xFCFD: return "Barrot Technology Co.,Ltd."; // 
				case 0xFCFC: return "Barrot Technology Co.,Ltd."; // 
				case 0xFCFB: return "Shenzhen Benwei Media Co., Ltd."; // 
				case 0xFCFA: return "Leupold & Stevens, Inc."; // 
				case 0xFCF9: return "Leupold & Stevens, Inc."; // 
				case 0xFCF8: return "Honor Device Co., Ltd."; // 
				case 0xFCF7: return "Honor Device Co., Ltd."; // 
				case 0xFCF6: return "The Linux Foundation"; // 
				case 0xFCF5: return "Trident Communication Technology, LLC"; // 
				case 0xFCF4: return "Allegion"; // 
				case 0xFCF3: return "Armatura LLC"; // 
				case 0xFCF2: return "Bitwards Oy"; // 
				case 0xFCF1: return "Google LLC"; // 
				case 0xFCF0: return "Security Enhancement Systems, LLC"; // 
				case 0xFCEF: return "Divesoft s.r.o."; // 
				case 0xFCEE: return "Velentium, LLC"; // 
				case 0xFCED: return "Workaround Gmbh"; // 
				case 0xFCEC: return "Griffwerk GmbH"; // 
				case 0xFCEB: return "Avi-On"; // 
				case 0xFCEA: return "Chess Wise B.V."; // 
				case 0xFCE9: return "MindRhythm, Inc."; // 
				case 0xFCE8: return "ITT Industries"; // 
				case 0xFCE7: return "TKH Security B.V."; // 
				case 0xFCE6: return "Guard RFID Solutions Inc."; // 
				case 0xFCE5: return "Samsara Networks, Inc"; // 
				case 0xFCE4: return "Samsara Networks, Inc"; // 
				case 0xFCE3: return "Smith & Nephew Medical Limited"; // 
				case 0xFCE2: return "Baracoda Daily Healthtech"; // 
				case 0xFCE1: return "Sony Group Corporation"; // 
				case 0xFCE0: return "Akciju sabiedriba 'SAF TEHNIKA'"; // 
				case 0xFCDF: return "NIO USA, Inc."; // 
				case 0xFCDE: return "ARCTOP, INC."; // 
				case 0xFCDD: return "Mobilaris AB"; // 
				case 0xFCDC: return "Amazon.com Services, LLC"; // 
				case 0xFCDB: return "aconno GmbH"; // 
				case 0xFCDA: return "Draeger"; // 
				case 0xFCD9: return "Huso, INC"; // 
				case 0xFCD8: return "Appex Factory S.L."; // 
				case 0xFCD7: return "PowerPal Pty Ltd"; // 
				case 0xFCD6: return "SWISSINNO SOLUTIONS AG"; // 
				case 0xFCD5: return "Nortek Security & Control"; // 
				case 0xFCD4: return "OMRON HEALTHCARE"; // 
				case 0xFCD3: return "Fisher & Paykel Healthcare"; // 
				case 0xFCD2: return "Allterco Robotics ltd"; // 
				case 0xFCD1: return "Shenzhen Benwei Media Co.,Ltd."; // 
				case 0xFCD0: return "Laerdal Medical AS"; // 
				case 0xFCCF: return "Google LLC"; // 
				case 0xFCCE: return "Luna Health, Inc."; // 
				case 0xFCCD: return "Marshall Group AB"; // 
				case 0xFCCB: return "TOTO LTD."; // 
				case 0xFCCA: return "Cosmed s.r.l."; // 
				case 0xFCC9: return "SkyHawke Technologies"; // 
				case 0xFCC8: return "Allthenticate, Inc."; // 
				case 0xFCC7: return "PB INC."; // 
				case 0xFCC6: return "Wiliot LTD."; // 
				case 0xFCC5: return "OMRON(DALIAN) CO,.LTD."; // 
				case 0xFCC4: return "OMRON(DALIAN) CO,.LTD."; // 
				case 0xFCC3: return "HP Inc."; // 
				case 0xFCC2: return "Qualcomm Technologies, Inc."; // 
				case 0xFCC1: return "TIMECODE SYSTEMS LIMITED"; // 
				case 0xFCC0: return "Xiaomi Inc."; // 
				case 0xFCBF: return "ASSA ABLOY Opening Solutions Sweden AB"; // 
				case 0xFCBE: return "Musen Connect, Inc."; // 
				case 0xFCBD: return "Toshiba Corporation"; // 
				case 0xFCBC: return "Drowsy Digital, Inc."; // 
				case 0xFCBB: return "SharkNinja Operating LLC"; // 
				case 0xFCBA: return "BlueID GmbH"; // 
				case 0xFCB9: return "Lumi United Technology Co., Ltd"; // 
				case 0xFCB8: return "Ribbiot, INC."; // 
				case 0xFCB7: return "T-Mobile USA"; // 
				case 0xFCB6: return "OMRON HEALTHCARE Co., Ltd."; // 
				case 0xFCB5: return "OMRON HEALTHCARE Co., Ltd."; // 
				case 0xFCB4: return "OMRON HEALTHCARE Co., Ltd."; // 
				case 0xFCB3: return "SWEEN"; // 
				case 0xFCB2: return "Apple Inc."; // 
				case 0xFCB1: return "Google LLC"; // 
				case 0xFCB0: return "Ford Motor Company"; // 
				case 0xFCAF: return "AltoBeam Inc."; // 
				case 0xFCAE: return "Imagine Marketing Limited"; // 
				case 0xFCAD: return "Beijing 99help Safety Technology Co., Ltd"; // 
				case 0xFCAC: return "IRISS INC."; // 
				case 0xFCAB: return "IRISS INC."; // 
				case 0xFCAA: return "Spintly, Inc."; // 
				case 0xFCA9: return "Medtronic Inc."; // 
				case 0xFCA8: return "Medtronic Inc."; // 
				case 0xFCA7: return "Hubble Network Inc."; // 
				case 0xFCA6: return "Hubble Network Inc."; // 
				case 0xFCA5: return "HAYWARD INDUSTRIES, INC."; // 
				case 0xFCA4: return "HP Inc."; // 
				case 0xFCA3: return "Gunnebo Aktiebolag"; // 
				case 0xFCA2: return "Meizu Technology Co., Ltd."; // 
				case 0xFCA1: return "PF SCHWEISSTECHNOLOGIE GMBH"; // 
				case 0xFCA0: return "Apple Inc."; // 
				case 0xFC9F: return "Delta Development Team, Inc"; // 
				case 0xFC9E: return "Dell Computer Corporation"; // 
				case 0xFC9D: return "Lenovo (Singapore) Pte Ltd."; // 
				case 0xFC9B: return "Merry Electronics (S) Pte Ltd"; // 
				case 0xFC9A: return "Koppli AB"; // 
				case 0xFC99: return "Badger Meter"; // 
				case 0xFC98: return "Ruuvi Innovations Ltd."; // 
				case 0xFC97: return "Japan Display Inc."; // 
				case 0xFC96: return "LEGO System A/S"; // 
				case 0xFC95: return "Hippo Camp Software Ltd."; // 
				case 0xFC94: return "Apple Inc."; // 
				case 0xFC93: return "Komatsu Ltd."; // 
				case 0xFC92: return "Furuno Electric Co., Ltd."; // 
				case 0xFC91: return "Samsung Electronics Co., Ltd."; // 
				case 0xFC90: return "Wiliot LTD."; // 
				case 0xFC8F: return "Bose Corporation"; // 
				case 0xFC8E: return "Blue Iris Labs, Inc."; // 
				case 0xFC8D: return "Caire Inc."; // 
				case 0xFC8C: return "VusionGroup"; // 
				case 0xFC8B: return "Kaspersky Lab Middle East FZ-LLC"; // 
				case 0xFC8A: return "Intel Corporation"; // 
				case 0xFC89: return "Intel Corporation"; // 
				case 0xFC88: return "CCC del Uruguay"; // 
				case 0xFC87: return "Samsara Networks, Inc"; // 
				case 0xFC86: return "Samsara Networks, Inc"; // 
				case 0xFC85: return "Zhejiang Huanfu Technology Co., LTD"; // 
				case 0xFC84: return "NINGBO FOTILE KITCHENWARE CO., LTD."; // 
				case 0xFC83: return "iHealth Labs, Inc."; // 
				case 0xFC82: return "Zwift, Inc."; // 
				case 0xFC81: return "Axon Enterprise, Inc."; // 
				case 0xFC80: return "TELE System Communications Pte. Ltd."; // 
				case 0xFC7F: return "Southco"; // 
				case 0xFC7E: return "Harman International"; // 
				case 0xFC7D: return "MML US, Inc"; // 
				case 0xFC7C: return "Motorola Mobility, LLC"; // 
				case 0xFC7B: return "Testo SE & Co. KGaA"; // 
				case 0xFC7A: return "Outshiny India Private Limited"; // 
				case 0xFC79: return "LG Electronics Inc."; // 
				case 0xFC78: return "DHL"; // 
				case 0xFC77: return "SING SUN TECHNOLOGY (INTERNATIONAL) LIMITED"; // 
				case 0xFC76: return "Weber-Stephen Products LLC"; // 
				case 0xFC75: return "Xiaomi Inc."; // 
				case 0xFC74: return "EMBEINT INC"; // 
				case 0xFC73: return "Google LLC"; // 
				case 0xFC72: return "iodyne, LLC"; // 
				case 0xFC71: return "Hive-Zox International SA"; // 
				case 0xFC70: return "MOTIVE TECHNOLOGIES, INC."; // 
				case 0xFC6F: return "NextSense, Inc."; // 
				case 0xFC6E: return "stryker"; // 
				case 0xFC6D: return "MOTIVE TECHNOLOGIES, INC."; // 
				case 0xFC6C: return "Powerstick.com"; // 
				case 0xFC6B: return "Sonos Inc"; // 
				case 0xFC6A: return "Sonos Inc"; // 
				case 0xFC69: return "Harman International"; // 
				case 0xFC68: return "RIGH, INC."; // 
				case 0xFC67: return "Guangdong Hengqin Xingtong Technology Co.,ltd."; // 
				case 0xFC66: return "Xiaomi Inc."; // 
				case 0xFC65: return "Robor Electronics B.V."; // 
				case 0xFC64: return "Volvo Technology AB"; // 
				case 0xFC63: return "Volvo Technology AB"; // 
				case 0xFC62: return "SPRiNTUS GmbH"; // 
				case 0xFC61: return "QIKCONNEX LLC"; // 
				case 0xFC60: return "Ohme Operations UK Limited"; // 
				case 0xFC5F: return "PI-CRYSTAL INC."; // 
				case 0xFC5E: return "KUBU SMART LIMITED"; // 
				case 0xFC5D: return "GP Acoustics International Limited"; // 
				case 0xFC5C: return "PLASTIC RESEARCH AND DEVELOPMENT CORPORATION"; // 
				case 0xFC5B: return "Time Location Systems AS"; // 
				case 0xFC5A: return "LAST LOCK INC."; // 
				case 0xFC59: return "Ant Group Co., Ltd."; // 
				case 0xFC58: return "Shenzhen Minew Technologies Co., Ltd."; // 
				case 0xFC57: return "Ambient Life Inc."; // 
				case 0xFC56: return "Google LLC"; // 
				case 0xFC55: return "BYD Company Limited"; // 
				case 0xFC54: return "Shenzhen Yinwang Intelligent Technologies Co., Ltd."; // 
				case 0xFC53: return "LEGIC Identsystems AG"; // 
				case 0xFC52: return "LG Electronics Inc."; // 
				case 0xFC51: return "Ant Group Co., Ltd."; // 
				case 0xFC50: return "Ant Group Co., Ltd."; // 
				case 0xFC4F: return "WaveRF, Corp."; // 
				case 0xFC4E: return "Lodestar Technology Inc."; // 
				case 0xFC4D: return "Lodestar Technology Inc."; // 
				case 0xFC4C: return "HP Inc."; // 
				case 0xFC4B: return "WinMagic Inc."; // 
				case 0xFC4A: return "Shenzhen Shokz Co.,Ltd."; // 
				case 0xFC49: return "Golioth, Inc."; // 
				case 0xFC48: return "Michelin"; // 
				case 0xFC47: return "Shanghai Ingeek Technology Co., Ltd."; // 
				case 0xFC46: return "Xiaomi"; // 
				case 0xFC45: return "Mitsubishi Electric Corporation"; // 
				case 0xFC44: return "Block, Inc."; // 
				case 0xFC43: return "Atmosic Technologies, Inc."; // 
				case 0xFC41: return "Yoto Limited"; // 
				case 0xFC3F: return "Milwaukee Electric Tools"; // 
				case 0xFC3E: return "Google LLC"; // 
				case 0xFC3D: return "Reelables, Inc."; // 
				case 0xFC3C: return "Eforthink Technology Co., Ltd."; // 
				case 0xFC3B: return "BONX INC."; // 
				case 0xFC3A: return "C.O.B.O. SpA"; // 
				case 0xFC39: return "Harman International"; // 
				case 0xFC38: return "SetPoint Medical"; // 
				case 0xFC37: return "TANlock GmbH"; // 
				case 0xFC36: return "PharmaSens AG"; // 
				case 0xFC35: return "Metabowerke GmbH"; // 
				case 0xFC34: return "Mitsubishi Electric Corporation"; // 
				case 0xFC32: return "InPlay, Inc."; // 
				case 0xFC31: return "Ford Motor Company"; // 
				case 0xFC30: return "Arashi Vision Inc."; // 
				case 0xFC2C: return "Verkada Inc."; // 
				case 0xFC2B: return "Elder Technologies, Inc"; // 
				case 0xFC2A: return "Troo Corporation"; // 
            // endupdatefile
