using System;

namespace BluetoothConversions
{
    static class BluetoothCharacteristic
    {
        public static string Decode(UInt16 value)
        {
            // From https://btprodspecificationrefs.blob.core.windows.net/assigned-values/16-bit%20UUID%20Numbers%20Document.pdf
            // the items marked GATT Characterisic and Object Type
            switch (value)
            {
                // updatefile:
                // url:https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/uuids/characteristic_uuids.yaml
                // file:characteristic_uuids.yaml
                // startupdatefile:
				case 0x2A00: return "Device Name"; // org.bluetooth.characteristic.gap.device_name
				case 0x2A01: return "Appearance"; // org.bluetooth.characteristic.gap.appearance
				case 0x2A02: return "Peripheral Privacy Flag"; // org.bluetooth.characteristic.gap.peripheral_privacy_flag
				case 0x2A03: return "Reconnection Address"; // org.bluetooth.characteristic.gap.reconnection_address
				case 0x2A04: return "Peripheral Preferred Connection Parameters"; // org.bluetooth.characteristic.gap.peripheral_preferred_connection_parameters
				case 0x2A05: return "Service Changed"; // org.bluetooth.characteristic.gatt.service_changed
				case 0x2A06: return "Alert Level"; // org.bluetooth.characteristic.alert_level
				case 0x2A07: return "Tx Power Level"; // org.bluetooth.characteristic.tx_power_level
				case 0x2A08: return "Date Time"; // org.bluetooth.characteristic.date_time
				case 0x2A09: return "Day of Week"; // org.bluetooth.characteristic.day_of_week
				case 0x2A0A: return "Day Date Time"; // org.bluetooth.characteristic.day_date_time
				case 0x2A0C: return "Exact Time 256"; // org.bluetooth.characteristic.exact_time_256
				case 0x2A0D: return "DST Offset"; // org.bluetooth.characteristic.dst_offset
				case 0x2A0E: return "Time Zone"; // org.bluetooth.characteristic.time_zone
				case 0x2A0F: return "Local Time Information"; // org.bluetooth.characteristic.local_time_information
				case 0x2A11: return "Time with DST"; // org.bluetooth.characteristic.time_with_dst
				case 0x2A12: return "Time Accuracy"; // org.bluetooth.characteristic.time_accuracy
				case 0x2A13: return "Time Source"; // org.bluetooth.characteristic.time_source
				case 0x2A14: return "Reference Time Information"; // org.bluetooth.characteristic.reference_time_information
				case 0x2A16: return "Time Update Control Point"; // org.bluetooth.characteristic.time_update_control_point
				case 0x2A17: return "Time Update State"; // org.bluetooth.characteristic.time_update_state
				case 0x2A18: return "Glucose Measurement"; // org.bluetooth.characteristic.glucose_measurement
				case 0x2A19: return "Battery Level"; // org.bluetooth.characteristic.battery_level
				case 0x2A1C: return "Temperature Measurement"; // org.bluetooth.characteristic.temperature_measurement
				case 0x2A1D: return "Temperature Type"; // org.bluetooth.characteristic.temperature_type
				case 0x2A1E: return "Intermediate Temperature"; // org.bluetooth.characteristic.intermediate_temperature
				case 0x2A21: return "Measurement Interval"; // org.bluetooth.characteristic.measurement_interval
				case 0x2A22: return "Boot Keyboard Input Report"; // org.bluetooth.characteristic.boot_keyboard_input_report
				case 0x2A23: return "System ID"; // org.bluetooth.characteristic.system_id
				case 0x2A24: return "Model Number String"; // org.bluetooth.characteristic.model_number_string
				case 0x2A25: return "Serial Number String"; // org.bluetooth.characteristic.serial_number_string
				case 0x2A26: return "Firmware Revision String"; // org.bluetooth.characteristic.firmware_revision_string
				case 0x2A27: return "Hardware Revision String"; // org.bluetooth.characteristic.hardware_revision_string
				case 0x2A28: return "Software Revision String"; // org.bluetooth.characteristic.software_revision_string
				case 0x2A29: return "Manufacturer Name String"; // org.bluetooth.characteristic.manufacturer_name_string
				case 0x2A2A: return "IEEE 11073-20601 Regulatory Certification Data List"; // org.bluetooth.characteristic.ieee_11073_20601_regulatory_certification_data_list
				case 0x2A2B: return "Current Time"; // org.bluetooth.characteristic.current_time
				case 0x2A2C: return "Magnetic Declination"; // org.bluetooth.characteristic.magnetic_declination
				case 0x2A31: return "Scan Refresh"; // org.bluetooth.characteristic.scan_refresh
				case 0x2A32: return "Boot Keyboard Output Report"; // org.bluetooth.characteristic.boot_keyboard_output_report
				case 0x2A33: return "Boot Mouse Input Report"; // org.bluetooth.characteristic.boot_mouse_input_report
				case 0x2A34: return "Glucose Measurement Context"; // org.bluetooth.characteristic.glucose_measurement_context
				case 0x2A35: return "Blood Pressure Measurement"; // org.bluetooth.characteristic.blood_pressure_measurement
				case 0x2A36: return "Intermediate Cuff Pressure"; // org.bluetooth.characteristic.intermediate_cuff_pressure
				case 0x2A37: return "Heart Rate Measurement"; // org.bluetooth.characteristic.heart_rate_measurement
				case 0x2A38: return "Body Sensor Location"; // org.bluetooth.characteristic.body_sensor_location
				case 0x2A39: return "Heart Rate Control Point"; // org.bluetooth.characteristic.heart_rate_control_point
				case 0x2A3F: return "Alert Status"; // org.bluetooth.characteristic.alert_status
				case 0x2A40: return "Ringer Control Point"; // org.bluetooth.characteristic.ringer_control_point
				case 0x2A41: return "Ringer Setting"; // org.bluetooth.characteristic.ringer_setting
				case 0x2A42: return "Alert Category ID Bit Mask"; // org.bluetooth.characteristic.alert_category_id_bit_mask
				case 0x2A43: return "Alert Category ID"; // org.bluetooth.characteristic.alert_category_id
				case 0x2A44: return "Alert Notification Control Point"; // org.bluetooth.characteristic.alert_notification_control_point
				case 0x2A45: return "Unread Alert Status"; // org.bluetooth.characteristic.unread_alert_status
				case 0x2A46: return "New Alert"; // org.bluetooth.characteristic.new_alert
				case 0x2A47: return "Supported New Alert Category"; // org.bluetooth.characteristic.supported_new_alert_category
				case 0x2A48: return "Supported Unread Alert Category"; // org.bluetooth.characteristic.supported_unread_alert_category
				case 0x2A49: return "Blood Pressure Feature"; // org.bluetooth.characteristic.blood_pressure_feature
				case 0x2A4A: return "HID Information"; // org.bluetooth.characteristic.hid_information
				case 0x2A4B: return "Report Map"; // org.bluetooth.characteristic.report_map
				case 0x2A4C: return "HID Control Point"; // org.bluetooth.characteristic.hid_control_point
				case 0x2A4D: return "Report"; // org.bluetooth.characteristic.report
				case 0x2A4E: return "Protocol Mode"; // org.bluetooth.characteristic.protocol_mode
				case 0x2A4F: return "Scan Interval Window"; // org.bluetooth.characteristic.scan_interval_window
				case 0x2A50: return "PnP ID"; // org.bluetooth.characteristic.pnp_id
				case 0x2A51: return "Glucose Feature"; // org.bluetooth.characteristic.glucose_feature
				case 0x2A52: return "Record Access Control Point"; // org.bluetooth.characteristic.record_access_control_point
				case 0x2A53: return "RSC Measurement"; // org.bluetooth.characteristic.rsc_measurement
				case 0x2A54: return "RSC Feature"; // org.bluetooth.characteristic.rsc_feature
				case 0x2A55: return "SC Control Point"; // org.bluetooth.characteristic.sc_control_point
				case 0x2A5A: return "Aggregate"; // org.bluetooth.characteristic.aggregate
				case 0x2A5B: return "CSC Measurement"; // org.bluetooth.characteristic.csc_measurement
				case 0x2A5C: return "CSC Feature"; // org.bluetooth.characteristic.csc_feature
				case 0x2A5D: return "Sensor Location"; // org.bluetooth.characteristic.sensor_location
				case 0x2A5E: return "PLX Spot-Check Measurement"; // org.bluetooth.characteristic.plx_spot_check_measurement
				case 0x2A5F: return "PLX Continuous Measurement"; // org.bluetooth.characteristic.plx_continuous_measurement
				case 0x2A60: return "PLX Features"; // org.bluetooth.characteristic.plx_features
				case 0x2A63: return "Cycling Power Measurement"; // org.bluetooth.characteristic.cycling_power_measurement
				case 0x2A64: return "Cycling Power Vector"; // org.bluetooth.characteristic.cycling_power_vector
				case 0x2A65: return "Cycling Power Feature"; // org.bluetooth.characteristic.cycling_power_feature
				case 0x2A66: return "Cycling Power Control Point"; // org.bluetooth.characteristic.cycling_power_control_point
				case 0x2A67: return "Location and Speed"; // org.bluetooth.characteristic.location_and_speed
				case 0x2A68: return "Navigation"; // org.bluetooth.characteristic.navigation
				case 0x2A69: return "Position Quality"; // org.bluetooth.characteristic.position_quality
				case 0x2A6A: return "LN Feature"; // org.bluetooth.characteristic.ln_feature
				case 0x2A6B: return "LN Control Point"; // org.bluetooth.characteristic.ln_control_point
				case 0x2A6C: return "Elevation"; // org.bluetooth.characteristic.elevation
				case 0x2A6D: return "Pressure"; // org.bluetooth.characteristic.pressure
				case 0x2A6E: return "Temperature"; // org.bluetooth.characteristic.temperature
				case 0x2A6F: return "Humidity"; // org.bluetooth.characteristic.humidity
				case 0x2A70: return "True Wind Speed"; // org.bluetooth.characteristic.true_wind_speed
				case 0x2A71: return "True Wind Direction"; // org.bluetooth.characteristic.true_wind_direction
				case 0x2A72: return "Apparent Wind Speed"; // org.bluetooth.characteristic.apparent_wind_speed
				case 0x2A73: return "Apparent Wind Direction"; // org.bluetooth.characteristic.apparent_wind_direction
				case 0x2A74: return "Gust Factor"; // org.bluetooth.characteristic.gust_factor
				case 0x2A75: return "Pollen Concentration"; // org.bluetooth.characteristic.pollen_concentration
				case 0x2A76: return "UV Index"; // org.bluetooth.characteristic.uv_index
				case 0x2A77: return "Irradiance"; // org.bluetooth.characteristic.irradiance
				case 0x2A78: return "Rainfall"; // org.bluetooth.characteristic.rainfall
				case 0x2A79: return "Wind Chill"; // org.bluetooth.characteristic.wind_chill
				case 0x2A7A: return "Heat Index"; // org.bluetooth.characteristic.heat_index
				case 0x2A7B: return "Dew Point"; // org.bluetooth.characteristic.dew_point
				case 0x2A7D: return "Descriptor Value Changed"; // org.bluetooth.characteristic.descriptor_value_changed
				case 0x2A7E: return "Aerobic Heart Rate Lower Limit"; // org.bluetooth.characteristic.aerobic_heart_rate_lower_limit
				case 0x2A7F: return "Aerobic Threshold"; // org.bluetooth.characteristic.aerobic_threshold
				case 0x2A80: return "Age"; // org.bluetooth.characteristic.age
				case 0x2A81: return "Anaerobic Heart Rate Lower Limit"; // org.bluetooth.characteristic.anaerobic_heart_rate_lower_limit
				case 0x2A82: return "Anaerobic Heart Rate Upper Limit"; // org.bluetooth.characteristic.anaerobic_heart_rate_upper_limit
				case 0x2A83: return "Anaerobic Threshold"; // org.bluetooth.characteristic.anaerobic_threshold
				case 0x2A84: return "Aerobic Heart Rate Upper Limit"; // org.bluetooth.characteristic.aerobic_heart_rate_upper_limit
				case 0x2A85: return "Date of Birth"; // org.bluetooth.characteristic.date_of_birth
				case 0x2A86: return "Date of Threshold Assessment"; // org.bluetooth.characteristic.date_of_threshold_assessment
				case 0x2A87: return "Email Address"; // org.bluetooth.characteristic.email_address
				case 0x2A88: return "Fat Burn Heart Rate Lower Limit"; // org.bluetooth.characteristic.fat_burn_heart_rate_lower_limit
				case 0x2A89: return "Fat Burn Heart Rate Upper Limit"; // org.bluetooth.characteristic.fat_burn_heart_rate_upper_limit
				case 0x2A8A: return "First Name"; // org.bluetooth.characteristic.first_name
				case 0x2A8B: return "Five Zone Heart Rate Limits"; // org.bluetooth.characteristic.five_zone_heart_rate_limits
				case 0x2A8C: return "Gender"; // org.bluetooth.characteristic.gender
				case 0x2A8D: return "Heart Rate Max"; // org.bluetooth.characteristic.heart_rate_max
				case 0x2A8E: return "Height"; // org.bluetooth.characteristic.height
				case 0x2A8F: return "Hip Circumference"; // org.bluetooth.characteristic.hip_circumference
				case 0x2A90: return "Last Name"; // org.bluetooth.characteristic.last_name
				case 0x2A91: return "Maximum Recommended Heart Rate"; // org.bluetooth.characteristic.maximum_recommended_heart_rate
				case 0x2A92: return "Resting Heart Rate"; // org.bluetooth.characteristic.resting_heart_rate
				case 0x2A93: return "Sport Type for Aerobic and Anaerobic Thresholds"; // org.bluetooth.characteristic.sport_type_for_aerobic_and_anaerobic_thresholds
				case 0x2A94: return "Three Zone Heart Rate Limits"; // org.bluetooth.characteristic.three_zone_heart_rate_limits
				case 0x2A95: return "Two Zone Heart Rate Limits"; // org.bluetooth.characteristic.two_zone_heart_rate_limits
				case 0x2A96: return "VO2 Max"; // org.bluetooth.characteristic.vo2_max
				case 0x2A97: return "Waist Circumference"; // org.bluetooth.characteristic.waist_circumference
				case 0x2A98: return "Weight"; // org.bluetooth.characteristic.weight
				case 0x2A99: return "Database Change Increment"; // org.bluetooth.characteristic.database_change_increment
				case 0x2A9A: return "User Index"; // org.bluetooth.characteristic.user_index
				case 0x2A9B: return "Body Composition Feature"; // org.bluetooth.characteristic.body_composition_feature
				case 0x2A9C: return "Body Composition Measurement"; // org.bluetooth.characteristic.body_composition_measurement
				case 0x2A9D: return "Weight Measurement"; // org.bluetooth.characteristic.weight_measurement
				case 0x2A9E: return "Weight Scale Feature"; // org.bluetooth.characteristic.weight_scale_feature
				case 0x2A9F: return "User Control Point"; // org.bluetooth.characteristic.user_control_point
				case 0x2AA0: return "Magnetic Flux Density - 2D"; // org.bluetooth.characteristic.magnetic_flux_density_2d
				case 0x2AA1: return "Magnetic Flux Density - 3D"; // org.bluetooth.characteristic.magnetic_flux_density_3d
				case 0x2AA2: return "Language"; // org.bluetooth.characteristic.language
				case 0x2AA3: return "Barometric Pressure Trend"; // org.bluetooth.characteristic.barometric_pressure_trend
				case 0x2AA4: return "Bond Management Control Point"; // org.bluetooth.characteristic.bond_management_control_point
				case 0x2AA5: return "Bond Management Feature"; // org.bluetooth.characteristic.bond_management_feature
				case 0x2AA6: return "Central Address Resolution"; // org.bluetooth.characteristic.gap.central_address_resolution
				case 0x2AA7: return "CGM Measurement"; // org.bluetooth.characteristic.cgm_measurement
				case 0x2AA8: return "CGM Feature"; // org.bluetooth.characteristic.cgm_feature
				case 0x2AA9: return "CGM Status"; // org.bluetooth.characteristic.cgm_status
				case 0x2AAA: return "CGM Session Start Time"; // org.bluetooth.characteristic.cgm_session_start_time
				case 0x2AAB: return "CGM Session Run Time"; // org.bluetooth.characteristic.cgm_session_run_time
				case 0x2AAC: return "CGM Specific Ops Control Point"; // org.bluetooth.characteristic.cgm_specific_ops_control_point
				case 0x2AAD: return "Indoor Positioning Configuration"; // org.bluetooth.characteristic.indoor_positioning_configuration
				case 0x2AAE: return "Latitude"; // org.bluetooth.characteristic.latitude
				case 0x2AAF: return "Longitude"; // org.bluetooth.characteristic.longitude
				case 0x2AB0: return "Local North Coordinate"; // org.bluetooth.characteristic.local_north_coordinate
				case 0x2AB1: return "Local East Coordinate"; // org.bluetooth.characteristic.local_east_coordinate
				case 0x2AB2: return "Floor Number"; // org.bluetooth.characteristic.floor_number
				case 0x2AB3: return "Altitude"; // org.bluetooth.characteristic.altitude
				case 0x2AB4: return "Uncertainty"; // org.bluetooth.characteristic.uncertainty
				case 0x2AB5: return "Location Name"; // org.bluetooth.characteristic.location_name
				case 0x2AB6: return "URI"; // org.bluetooth.characteristic.uri
				case 0x2AB7: return "HTTP Headers"; // org.bluetooth.characteristic.http_headers
				case 0x2AB8: return "HTTP Status Code"; // org.bluetooth.characteristic.http_status_code
				case 0x2AB9: return "HTTP Entity Body"; // org.bluetooth.characteristic.http_entity_body
				case 0x2ABA: return "HTTP Control Point"; // org.bluetooth.characteristic.http_control_point
				case 0x2ABB: return "HTTPS Security"; // org.bluetooth.characteristic.https_security
				case 0x2ABC: return "TDS Control Point"; // org.bluetooth.characteristic.tds_control_point
				case 0x2ABD: return "OTS Feature"; // org.bluetooth.characteristic.ots_feature
				case 0x2ABE: return "Object Name"; // org.bluetooth.characteristic.object_name
				case 0x2ABF: return "Object Type"; // org.bluetooth.characteristic.object_type
				case 0x2AC0: return "Object Size"; // org.bluetooth.characteristic.object_size
				case 0x2AC1: return "Object First-Created"; // org.bluetooth.characteristic.object_first_created
				case 0x2AC2: return "Object Last-Modified"; // org.bluetooth.characteristic.object_last_modified
				case 0x2AC3: return "Object ID"; // org.bluetooth.characteristic.object_id
				case 0x2AC4: return "Object Properties"; // org.bluetooth.characteristic.object_properties
				case 0x2AC5: return "Object Action Control Point"; // org.bluetooth.characteristic.object_action_control_point
				case 0x2AC6: return "Object List Control Point"; // org.bluetooth.characteristic.object_list_control_point
				case 0x2AC7: return "Object List Filter"; // org.bluetooth.characteristic.object_list_filter
				case 0x2AC8: return "Object Changed"; // org.bluetooth.characteristic.object_changed
				case 0x2AC9: return "Resolvable Private Address Only"; // org.bluetooth.characteristic.resolvable_private_address_only
				case 0x2ACC: return "Fitness Machine Feature"; // org.bluetooth.characteristic.fitness_machine_feature
				case 0x2ACD: return "Treadmill Data"; // org.bluetooth.characteristic.treadmill_data
				case 0x2ACE: return "Cross Trainer Data"; // org.bluetooth.characteristic.cross_trainer_data
				case 0x2ACF: return "Step Climber Data"; // org.bluetooth.characteristic.step_climber_data
				case 0x2AD0: return "Stair Climber Data"; // org.bluetooth.characteristic.stair_climber_data
				case 0x2AD1: return "Rower Data"; // org.bluetooth.characteristic.rower_data
				case 0x2AD2: return "Indoor Bike Data"; // org.bluetooth.characteristic.indoor_bike_data
				case 0x2AD3: return "Training Status"; // org.bluetooth.characteristic.training_status
				case 0x2AD4: return "Supported Speed Range"; // org.bluetooth.characteristic.supported_speed_range
				case 0x2AD5: return "Supported Inclination Range"; // org.bluetooth.characteristic.supported_inclination_range
				case 0x2AD6: return "Supported Resistance Level Range"; // org.bluetooth.characteristic.supported_resistance_level_range
				case 0x2AD7: return "Supported Heart Rate Range"; // org.bluetooth.characteristic.supported_heart_rate_range
				case 0x2AD8: return "Supported Power Range"; // org.bluetooth.characteristic.supported_power_range
				case 0x2AD9: return "Fitness Machine Control Point"; // org.bluetooth.characteristic.fitness_machine_control_point
				case 0x2ADA: return "Fitness Machine Status"; // org.bluetooth.characteristic.fitness_machine_status
				case 0x2ADB: return "Mesh Provisioning Data In"; // org.bluetooth.characteristic.mesh_provisioning_data_in
				case 0x2ADC: return "Mesh Provisioning Data Out"; // org.bluetooth.characteristic.mesh_provisioning_data_out
				case 0x2ADD: return "Mesh Proxy Data In"; // org.bluetooth.characteristic.mesh_proxy_data_in
				case 0x2ADE: return "Mesh Proxy Data Out"; // org.bluetooth.characteristic.mesh_proxy_data_out
				case 0x2AE0: return "Average Current"; // org.bluetooth.characteristic.average_current
				case 0x2AE1: return "Average Voltage"; // org.bluetooth.characteristic.average_voltage
				case 0x2AE2: return "Boolean"; // org.bluetooth.characteristic.boolean
				case 0x2AE3: return "Chromatic Distance from Planckian"; // org.bluetooth.characteristic.chromatic_distance_from_planckian
				case 0x2AE4: return "Chromaticity Coordinates"; // org.bluetooth.characteristic.chromaticity_coordinates
				case 0x2AE5: return "Chromaticity in CCT and Duv Values"; // org.bluetooth.characteristic.chromaticity_in_cct_and_duv_values
				case 0x2AE6: return "Chromaticity Tolerance"; // org.bluetooth.characteristic.chromaticity_tolerance
				case 0x2AE7: return "CIE 13.3-1995 Color Rendering Index"; // org.bluetooth.characteristic.cie_13_3_1995_color_rendering_index
				case 0x2AE8: return "Coefficient"; // org.bluetooth.characteristic.coefficient
				case 0x2AE9: return "Correlated Color Temperature"; // org.bluetooth.characteristic.correlated_color_temperature
				case 0x2AEA: return "Count 16"; // org.bluetooth.characteristic.count_16
				case 0x2AEB: return "Count 24"; // org.bluetooth.characteristic.count_24
				case 0x2AEC: return "Country Code"; // org.bluetooth.characteristic.country_code
				case 0x2AED: return "Date UTC"; // org.bluetooth.characteristic.date_utc
				case 0x2AEE: return "Electric Current"; // org.bluetooth.characteristic.electric_current
				case 0x2AEF: return "Electric Current Range"; // org.bluetooth.characteristic.electric_current_range
				case 0x2AF0: return "Electric Current Specification"; // org.bluetooth.characteristic.electric_current_specification
				case 0x2AF1: return "Electric Current Statistics"; // org.bluetooth.characteristic.electric_current_statistics
				case 0x2AF2: return "Energy"; // org.bluetooth.characteristic.energy
				case 0x2AF3: return "Energy in a Period of Day"; // org.bluetooth.characteristic.energy_in_a_period_of_day
				case 0x2AF4: return "Event Statistics"; // org.bluetooth.characteristic.event_statistics
				case 0x2AF5: return "Fixed String 16"; // org.bluetooth.characteristic.fixed_string_16
				case 0x2AF6: return "Fixed String 24"; // org.bluetooth.characteristic.fixed_string_24
				case 0x2AF7: return "Fixed String 36"; // org.bluetooth.characteristic.fixed_string_36
				case 0x2AF8: return "Fixed String 8"; // org.bluetooth.characteristic.fixed_string_8
				case 0x2AF9: return "Generic Level"; // org.bluetooth.characteristic.generic_level
				case 0x2AFA: return "Global Trade Item Number"; // org.bluetooth.characteristic.global_trade_item_number
				case 0x2AFB: return "Illuminance"; // org.bluetooth.characteristic.illuminance
				case 0x2AFC: return "Luminous Efficacy"; // org.bluetooth.characteristic.luminous_efficacy
				case 0x2AFD: return "Luminous Energy"; // org.bluetooth.characteristic.luminous_energy
				case 0x2AFE: return "Luminous Exposure"; // org.bluetooth.characteristic.luminous_exposure
				case 0x2AFF: return "Luminous Flux"; // org.bluetooth.characteristic.luminous_flux
				case 0x2B00: return "Luminous Flux Range"; // org.bluetooth.characteristic.luminous_flux_range
				case 0x2B01: return "Luminous Intensity"; // org.bluetooth.characteristic.luminous_intensity
				case 0x2B02: return "Mass Flow"; // org.bluetooth.characteristic.mass_flow
				case 0x2B03: return "Perceived Lightness"; // org.bluetooth.characteristic.perceived_lightness
				case 0x2B04: return "Percentage 8"; // org.bluetooth.characteristic.percentage_8
				case 0x2B05: return "Power"; // org.bluetooth.characteristic.power
				case 0x2B06: return "Power Specification"; // org.bluetooth.characteristic.power_specification
				case 0x2B07: return "Relative Runtime in a Current Range"; // org.bluetooth.characteristic.relative_runtime_in_a_current_range
				case 0x2B08: return "Relative Runtime in a Generic Level Range"; // org.bluetooth.characteristic.relative_runtime_in_a_generic_level_range
				case 0x2B09: return "Relative Value in a Voltage Range"; // org.bluetooth.characteristic.relative_value_in_a_voltage_range
				case 0x2B0A: return "Relative Value in an Illuminance Range"; // org.bluetooth.characteristic.relative_value_in_an_illuminance_range
				case 0x2B0B: return "Relative Value in a Period of Day"; // org.bluetooth.characteristic.relative_value_in_a_period_of_day
				case 0x2B0C: return "Relative Value in a Temperature Range"; // org.bluetooth.characteristic.relative_value_in_a_temperature_range
				case 0x2B0D: return "Temperature 8"; // org.bluetooth.characteristic.temperature_8
				case 0x2B0E: return "Temperature 8 in a Period of Day"; // org.bluetooth.characteristic.temperature_8_in_a_period_of_day
				case 0x2B0F: return "Temperature 8 Statistics"; // org.bluetooth.characteristic.temperature_8_statistics
				case 0x2B10: return "Temperature Range"; // org.bluetooth.characteristic.temperature_range
				case 0x2B11: return "Temperature Statistics"; // org.bluetooth.characteristic.temperature_statistics
				case 0x2B12: return "Time Decihour 8"; // org.bluetooth.characteristic.time_decihour_8
				case 0x2B13: return "Time Exponential 8"; // org.bluetooth.characteristic.time_exponential_8
				case 0x2B14: return "Time Hour 24"; // org.bluetooth.characteristic.time_hour_24
				case 0x2B15: return "Time Millisecond 24"; // org.bluetooth.characteristic.time_millisecond_24
				case 0x2B16: return "Time Second 16"; // org.bluetooth.characteristic.time_second_16
				case 0x2B17: return "Time Second 8"; // org.bluetooth.characteristic.time_second_8
				case 0x2B18: return "Voltage"; // org.bluetooth.characteristic.voltage
				case 0x2B19: return "Voltage Specification"; // org.bluetooth.characteristic.voltage_specification
				case 0x2B1A: return "Voltage Statistics"; // org.bluetooth.characteristic.voltage_statistics
				case 0x2B1B: return "Volume Flow"; // org.bluetooth.characteristic.volume_flow
				case 0x2B1C: return "Chromaticity Coordinate"; // org.bluetooth.characteristic.chromaticity_coordinate
				case 0x2B1D: return "RC Feature"; // org.bluetooth.characteristic.rc_feature
				case 0x2B1E: return "RC Settings"; // org.bluetooth.characteristic.rc_settings
				case 0x2B1F: return "Reconnection Configuration Control Point"; // org.bluetooth.characteristic.reconnection_configuration_control_point
				case 0x2B20: return "IDD Status Changed"; // org.bluetooth.characteristic.idd_status_changed
				case 0x2B21: return "IDD Status"; // org.bluetooth.characteristic.idd_status
				case 0x2B22: return "IDD Annunciation Status"; // org.bluetooth.characteristic.idd_annunciation_status
				case 0x2B23: return "IDD Features"; // org.bluetooth.characteristic.idd_features
				case 0x2B24: return "IDD Status Reader Control Point"; // org.bluetooth.characteristic.idd_status_reader_control_point
				case 0x2B25: return "IDD Command Control Point"; // org.bluetooth.characteristic.idd_command_control_point
				case 0x2B26: return "IDD Command Data"; // org.bluetooth.characteristic.idd_command_data
				case 0x2B27: return "IDD Record Access Control Point"; // org.bluetooth.characteristic.idd_record_access_control_point
				case 0x2B28: return "IDD History Data"; // org.bluetooth.characteristic.idd_history_data
				case 0x2B29: return "Client Supported Features"; // org.bluetooth.characteristic.client_supported_features
				case 0x2B2A: return "Database Hash"; // org.bluetooth.characteristic.database_hash
				case 0x2B2B: return "BSS Control Point"; // org.bluetooth.characteristic.bss_control_point
				case 0x2B2C: return "BSS Response"; // org.bluetooth.characteristic.bss_response
				case 0x2B2D: return "Emergency ID"; // org.bluetooth.characteristic.emergency_id
				case 0x2B2E: return "Emergency Text"; // org.bluetooth.characteristic.emergency_text
				case 0x2B2F: return "ACS Status"; // org.bluetooth.characteristic.acs_status
				case 0x2B30: return "ACS Data In"; // org.bluetooth.characteristic.acs_data_in
				case 0x2B31: return "ACS Data Out Notify"; // org.bluetooth.characteristic.acs_data_out_notify
				case 0x2B32: return "ACS Data Out Indicate"; // org.bluetooth.characteristic.acs_data_out_indicate
				case 0x2B33: return "ACS Control Point"; // org.bluetooth.characteristic.acs_control_point
				case 0x2B34: return "Enhanced Blood Pressure Measurement"; // org.bluetooth.characteristic.enhanced_blood_pressure_measurement
				case 0x2B35: return "Enhanced Intermediate Cuff Pressure"; // org.bluetooth.characteristic.enhanced_intermediate_cuff_pressure
				case 0x2B36: return "Blood Pressure Record"; // org.bluetooth.characteristic.blood_pressure_record
				case 0x2B37: return "Registered User"; // org.bluetooth.characteristic.registered_user
				case 0x2B38: return "BR-EDR Handover Data"; // org.bluetooth.characteristic.br_edr_handover_data
				case 0x2B39: return "Bluetooth SIG Data"; // org.bluetooth.characteristic.bluetooth_sig_data
				case 0x2B3A: return "Server Supported Features"; // org.bluetooth.characteristic.server_supported_features
				case 0x2B3B: return "Physical Activity Monitor Features"; // org.bluetooth.characteristic.physical_activity_monitor_features
				case 0x2B3C: return "General Activity Instantaneous Data"; // org.bluetooth.characteristic.general_activity_instantaneous_data
				case 0x2B3D: return "General Activity Summary Data"; // org.bluetooth.characteristic.general_activity_summary_data
				case 0x2B3E: return "CardioRespiratory Activity Instantaneous Data"; // org.bluetooth.characteristic.cardiorespiratory_activity_instantaneous_data
				case 0x2B3F: return "CardioRespiratory Activity Summary Data"; // org.bluetooth.characteristic.cardiorespiratory_activity_summary_data
				case 0x2B40: return "Step Counter Activity Summary Data"; // org.bluetooth.characteristic.step_counter_activity_summary_data
				case 0x2B41: return "Sleep Activity Instantaneous Data"; // org.bluetooth.characteristic.sleep_activity_instantaneous_data
				case 0x2B42: return "Sleep Activity Summary Data"; // org.bluetooth.characteristic.sleep_activity_summary_data
				case 0x2B43: return "Physical Activity Monitor Control Point"; // org.bluetooth.characteristic.physical_activity_monitor_control_point
				case 0x2B44: return "Physical Activity Current Session"; // org.bluetooth.characteristic.physical_activity_current_session
				case 0x2B45: return "Physical Activity Session Descriptor"; // org.bluetooth.characteristic.physical_activity_session_descriptor
				case 0x2B46: return "Preferred Units"; // org.bluetooth.characteristic.preferred_units
				case 0x2B47: return "High Resolution Height"; // org.bluetooth.characteristic.high_resolution_height
				case 0x2B48: return "Middle Name"; // org.bluetooth.characteristic.middle_name
				case 0x2B49: return "Stride Length"; // org.bluetooth.characteristic.stride_length
				case 0x2B4A: return "Handedness"; // org.bluetooth.characteristic.handedness
				case 0x2B4B: return "Device Wearing Position"; // org.bluetooth.characteristic.device_wearing_position
				case 0x2B4C: return "Four Zone Heart Rate Limits"; // org.bluetooth.characteristic.four_zone_heart_rate_limits
				case 0x2B4D: return "High Intensity Exercise Threshold"; // org.bluetooth.characteristic.high_intensity_exercise_threshold
				case 0x2B4E: return "Activity Goal"; // org.bluetooth.characteristic.activity_goal
				case 0x2B4F: return "Sedentary Interval Notification"; // org.bluetooth.characteristic.sedentary_interval_notification
				case 0x2B50: return "Caloric Intake"; // org.bluetooth.characteristic.caloric_intake
				case 0x2B51: return "TMAP Role"; // org.bluetooth.characteristic.tmap_role
				case 0x2B77: return "Audio Input State"; // org.bluetooth.characteristic.audio_input_state
				case 0x2B78: return "Gain Settings Attribute"; // org.bluetooth.characteristic.gain_settings_attribute
				case 0x2B79: return "Audio Input Type"; // org.bluetooth.characteristic.audio_input_type
				case 0x2B7A: return "Audio Input Status"; // org.bluetooth.characteristic.audio_input_status
				case 0x2B7B: return "Audio Input Control Point"; // org.bluetooth.characteristic.audio_input_control_point
				case 0x2B7C: return "Audio Input Description"; // org.bluetooth.characteristic.audio_input_description
				case 0x2B7D: return "Volume State"; // org.bluetooth.characteristic.volume_state
				case 0x2B7E: return "Volume Control Point"; // org.bluetooth.characteristic.volume_control_point
				case 0x2B7F: return "Volume Flags"; // org.bluetooth.characteristic.volume_flags
				case 0x2B80: return "Volume Offset State"; // org.bluetooth.characteristic.volume_offset_state
				case 0x2B81: return "Audio Location"; // org.bluetooth.characteristic.audio_location
				case 0x2B82: return "Volume Offset Control Point"; // org.bluetooth.characteristic.volume_offset_control_point
				case 0x2B83: return "Audio Output Description"; // org.bluetooth.characteristic.audio_output_description
				case 0x2B84: return "Set Identity Resolving Key"; // org.bluetooth.characteristic.set_identity_resolving_key
				case 0x2B85: return "Coordinated Set Size"; // org.bluetooth.characteristic.size_characteristic
				case 0x2B86: return "Set Member Lock"; // org.bluetooth.characteristic.lock_characteristic
				case 0x2B87: return "Set Member Rank"; // org.bluetooth.characteristic.rank_characteristic
				case 0x2B88: return "Encrypted Data Key Material"; // org.bluetooth.characteristic.encrypted_data_key_material
				case 0x2B89: return "Apparent Energy 32"; // org.bluetooth.characteristic.apparent_energy_32
				case 0x2B8A: return "Apparent Power"; // org.bluetooth.characteristic.apparent_power
				case 0x2B8B: return "Live Health Observations"; // org.bluetooth.characteristic.live_health_observations
				case 0x2B8C: return "CO\textsubscript{2} Concentration"; // org.bluetooth.characteristic.co2_concentration
				case 0x2B8D: return "Cosine of the Angle"; // org.bluetooth.characteristic.cosine_of_the_angle
				case 0x2B8E: return "Device Time Feature"; // org.bluetooth.characteristic.device_time_feature
				case 0x2B8F: return "Device Time Parameters"; // org.bluetooth.characteristic.device_time_parameters
				case 0x2B90: return "Device Time"; // org.bluetooth.characteristic.device_time
				case 0x2B91: return "Device Time Control Point"; // org.bluetooth.characteristic.device_time_control_point
				case 0x2B92: return "Time Change Log Data"; // org.bluetooth.characteristic.time_change_log_data
				case 0x2B93: return "Media Player Name"; // org.bluetooth.characteristic.media_player_name
				case 0x2B94: return "Media Player Icon Object ID"; // org.bluetooth.characteristic.media_player_icon_object_id
				case 0x2B95: return "Media Player Icon URL"; // org.bluetooth.characteristic.media_player_icon_url
				case 0x2B96: return "Track Changed"; // org.bluetooth.characteristic.track_changed
				case 0x2B97: return "Track Title"; // org.bluetooth.characteristic.track_title
				case 0x2B98: return "Track Duration"; // org.bluetooth.characteristic.track_duration
				case 0x2B99: return "Track Position"; // org.bluetooth.characteristic.track_position
				case 0x2B9A: return "Playback Speed"; // org.bluetooth.characteristic.playback_speed
				case 0x2B9B: return "Seeking Speed"; // org.bluetooth.characteristic.seeking_speed
				case 0x2B9C: return "Current Track Segments Object ID"; // org.bluetooth.characteristic.current_track_segments_object_id
				case 0x2B9D: return "Current Track Object ID"; // org.bluetooth.characteristic.current_track_object_id
				case 0x2B9E: return "Next Track Object ID"; // org.bluetooth.characteristic.next_track_object_id
				case 0x2B9F: return "Parent Group Object ID"; // org.bluetooth.characteristic.parent_group_object_id
				case 0x2BA0: return "Current Group Object ID"; // org.bluetooth.characteristic.current_group_object_id
				case 0x2BA1: return "Playing Order"; // org.bluetooth.characteristic.playing_order
				case 0x2BA2: return "Playing Orders Supported"; // org.bluetooth.characteristic.playing_orders_supported
				case 0x2BA3: return "Media State"; // org.bluetooth.characteristic.media_state
				case 0x2BA4: return "Media Control Point"; // org.bluetooth.characteristic.media_control_point
				case 0x2BA5: return "Media Control Point Opcodes Supported"; // org.bluetooth.characteristic.media_control_point_opcodes_supported
				case 0x2BA6: return "Search Results Object ID"; // org.bluetooth.characteristic.search_results_object_id
				case 0x2BA7: return "Search Control Point"; // org.bluetooth.characteristic.search_control_point
				case 0x2BA8: return "Energy 32"; // org.bluetooth.characteristic.energy_32
				case 0x2BAD: return "Constant Tone Extension Enable"; // org.bluetooth.characteristic.constant_tone_extension_enable
				case 0x2BAE: return "Advertising Constant Tone Extension Minimum Length"; // org.bluetooth.characteristic.advertising_constant_tone_extension_minimum_length
				case 0x2BAF: return "Advertising Constant Tone Extension Minimum Transmit Count"; // org.bluetooth.characteristic.advertising_constant_tone_extension_minimum_transmit_count
				case 0x2BB0: return "Advertising Constant Tone Extension Transmit Duration"; // org.bluetooth.characteristic.advertising_constant_tone_extension_transmit_duration
				case 0x2BB1: return "Advertising Constant Tone Extension Interval"; // org.bluetooth.characteristic.advertising_constant_tone_extension_interval
				case 0x2BB2: return "Advertising Constant Tone Extension PHY"; // org.bluetooth.characteristic.advertising_constant_tone_extension_phy
				case 0x2BB3: return "Bearer Provider Name"; // org.bluetooth.characteristic.bearer_provider_name
				case 0x2BB4: return "Bearer UCI"; // org.bluetooth.characteristic.bearer_uci
				case 0x2BB5: return "Bearer Technology"; // org.bluetooth.characteristic.bearer_technology
				case 0x2BB6: return "Bearer URI Schemes Supported List"; // org.bluetooth.characteristic.bearer_uri_schemes_supported_list
				case 0x2BB7: return "Bearer Signal Strength"; // org.bluetooth.characteristic.bearer_signal_strength
				case 0x2BB8: return "Bearer Signal Strength Reporting Interval"; // org.bluetooth.characteristic.bearer_signal_strength_reporting_interval
				case 0x2BB9: return "Bearer List Current Calls"; // org.bluetooth.characteristic.bearer_list_current_calls
				case 0x2BBA: return "Content Control ID"; // org.bluetooth.characteristic.content_control_id
				case 0x2BBB: return "Status Flags"; // org.bluetooth.characteristic.status_flags
				case 0x2BBC: return "Incoming Call Target Bearer URI"; // org.bluetooth.characteristic.incoming_call_target_bearer_uri
				case 0x2BBD: return "Call State"; // org.bluetooth.characteristic.call_state
				case 0x2BBE: return "Call Control Point"; // org.bluetooth.characteristic.call_control_point
				case 0x2BBF: return "Call Control Point Optional Opcodes"; // org.bluetooth.characteristic.call_control_point_optional_opcodes
				case 0x2BC0: return "Termination Reason"; // org.bluetooth.characteristic.termination_reason
				case 0x2BC1: return "Incoming Call"; // org.bluetooth.characteristic.incoming_call
				case 0x2BC2: return "Call Friendly Name"; // org.bluetooth.characteristic.call_friendly_name
				case 0x2BC3: return "Mute"; // org.bluetooth.characteristic.mute
				case 0x2BC4: return "Sink ASE"; // org.bluetooth.characteristic.sink_ase
				case 0x2BC5: return "Source ASE"; // org.bluetooth.characteristic.source_ase
				case 0x2BC6: return "ASE Control Point"; // org.bluetooth.characteristic.ase_control_point
				case 0x2BC7: return "Broadcast Audio Scan Control Point"; // org.bluetooth.characteristic.broadcast_audio_scan_control_point
				case 0x2BC8: return "Broadcast Receive State"; // org.bluetooth.characteristic.broadcast_receive_state
				case 0x2BC9: return "Sink PAC"; // org.bluetooth.characteristic.sink_pac
				case 0x2BCA: return "Sink Audio Locations"; // org.bluetooth.characteristic.sink_audio_locations
				case 0x2BCB: return "Source PAC"; // org.bluetooth.characteristic.source_pac
				case 0x2BCC: return "Source Audio Locations"; // org.bluetooth.characteristic.source_audio_locations
				case 0x2BCD: return "Available Audio Contexts"; // org.bluetooth.characteristic.available_audio_contexts
				case 0x2BCE: return "Supported Audio Contexts"; // org.bluetooth.characteristic.supported_audio_contexts
				case 0x2BCF: return "Ammonia Concentration"; // org.bluetooth.characteristic.ammonia_concentration
				case 0x2BD0: return "Carbon Monoxide Concentration"; // org.bluetooth.characteristic.carbon_monoxide_concentration
				case 0x2BD1: return "Methane Concentration"; // org.bluetooth.characteristic.methane_concentration
				case 0x2BD2: return "Nitrogen Dioxide Concentration"; // org.bluetooth.characteristic.nitrogen_dioxide_concentration
				case 0x2BD3: return "Non-Methane Volatile Organic Compounds Concentration"; // org.bluetooth.characteristic.non-methane_volatile_organic_compounds_concentration
				case 0x2BD4: return "Ozone Concentration"; // org.bluetooth.characteristic.ozone_concentration
				case 0x2BD5: return "Particulate Matter - PM1 Concentration"; // org.bluetooth.characteristic.particulate_matter_pm1_concentration
				case 0x2BD6: return "Particulate Matter - PM2.5 Concentration"; // org.bluetooth.characteristic.particulate_matter_pm2_5_concentration
				case 0x2BD7: return "Particulate Matter - PM10 Concentration"; // org.bluetooth.characteristic.particulate_matter_pm10_concentration
				case 0x2BD8: return "Sulfur Dioxide Concentration"; // org.bluetooth.characteristic.sulfur_dioxide_concentration
				case 0x2BD9: return "Sulfur Hexafluoride Concentration"; // org.bluetooth.characteristic.sulfur_hexafluoride_concentration
				case 0x2BDA: return "Hearing Aid Features"; // org.bluetooth.characteristic.hearing_aid_features
				case 0x2BDB: return "Hearing Aid Preset Control Point"; // org.bluetooth.characteristic.hearing_aid_preset_control_point
				case 0x2BDC: return "Active Preset Index"; // org.bluetooth.characteristic.active_preset_index
				case 0x2BDD: return "Stored Health Observations"; // org.bluetooth.characteristic.stored_health_observations
				case 0x2BDE: return "Fixed String 64"; // org.bluetooth.characteristic.fixed_string_64
				case 0x2BDF: return "High Temperature"; // org.bluetooth.characteristic.high_temperature
				case 0x2BE0: return "High Voltage"; // org.bluetooth.characteristic.high_voltage
				case 0x2BE1: return "Light Distribution"; // org.bluetooth.characteristic.light_distribution
				case 0x2BE2: return "Light Output"; // org.bluetooth.characteristic.light_output
				case 0x2BE3: return "Light Source Type"; // org.bluetooth.characteristic.light_source_type
				case 0x2BE4: return "Noise"; // org.bluetooth.characteristic.noise
				case 0x2BE5: return "Relative Runtime in a Correlated Color Temperature Range"; // org.bluetooth.characteristic.relative_runtime_in_a_correlated_color_temperature_range
				case 0x2BE6: return "Time Second 32"; // org.bluetooth.characteristic.time_second_32
				case 0x2BE7: return "VOC Concentration"; // org.bluetooth.characteristic.voc_concentration
				case 0x2BE8: return "Voltage Frequency"; // org.bluetooth.characteristic.voltage_frequency
				case 0x2BE9: return "Battery Critical Status"; // org.bluetooth.characteristic.battery_critical_status
				case 0x2BEA: return "Battery Health Status"; // org.bluetooth.characteristic.battery_health_status
				case 0x2BEB: return "Battery Health Information"; // org.bluetooth.characteristic.battery_health_information
				case 0x2BEC: return "Battery Information"; // org.bluetooth.characteristic.battery_information
				case 0x2BED: return "Battery Level Status"; // org.bluetooth.characteristic.battery_level_status
				case 0x2BEE: return "Battery Time Status"; // org.bluetooth.characteristic.battery_time_status
				case 0x2BEF: return "Estimated Service Date"; // org.bluetooth.characteristic.estimated_service_date
				case 0x2BF0: return "Battery Energy Status"; // org.bluetooth.characteristic.battery_energy_status
				case 0x2BF1: return "Observation Schedule Changed"; // org.bluetooth.characteristic.observation_schedule_changed
				case 0x2BF2: return "Elapsed Time"; // org.bluetooth.characteristic.elapsed_time
				case 0x2BF3: return "Health Sensor Features"; // org.bluetooth.characteristic.health_sensor_features
				case 0x2BF4: return "GHS Control Point"; // org.bluetooth.characteristic.ghs_control_point
				case 0x2BF5: return "LE GATT Security Levels"; // org.bluetooth.characteristic.le_gatt_security_levels
				case 0x2BF6: return "ESL Address"; // org.bluetooth.characteristic.esl_address
				case 0x2BF7: return "AP Sync Key Material"; // org.bluetooth.characteristic.ap_sync_key_material
				case 0x2BF8: return "ESL Response Key Material"; // org.bluetooth.characteristic.esl_response_key_material
				case 0x2BF9: return "ESL Current Absolute Time"; // org.bluetooth.characteristic.esl_current_absolute_time
				case 0x2BFA: return "ESL Display Information"; // org.bluetooth.characteristic.esl_display_information
				case 0x2BFB: return "ESL Image Information"; // org.bluetooth.characteristic.esl_image_information
				case 0x2BFC: return "ESL Sensor Information"; // org.bluetooth.characteristic.esl_sensor_information
				case 0x2BFD: return "ESL LED Information"; // org.bluetooth.characteristic.esl_led_information
				case 0x2BFE: return "ESL Control Point"; // org.bluetooth.characteristic.esl_control_point
				case 0x2BFF: return "UDI for Medical Devices"; // org.bluetooth.characteristic.udi_for_medical_devices
				case 0x2C00: return "GMAP Role"; // org.bluetooth.characteristic.gmap_role
				case 0x2C01: return "UGG Features"; // org.bluetooth.characteristic.ugg_features
				case 0x2C02: return "UGT Features"; // org.bluetooth.characteristic.ugt_features
				case 0x2C03: return "BGS Features"; // org.bluetooth.characteristic.bgs_features
				case 0x2C04: return "BGR Features"; // org.bluetooth.characteristic.bgr_features
				case 0x2C05: return "Percentage 8 Steps"; // org.bluetooth.characteristic.percentage_8_steps
				case 0x2C06: return "Acceleration"; // org.bluetooth.characteristic.acceleration
				case 0x2C07: return "Force"; // org.bluetooth.characteristic.force
				case 0x2C08: return "Linear Position"; // org.bluetooth.characteristic.linear_position
				case 0x2C09: return "Rotational Speed"; // org.bluetooth.characteristic.rotational_speed
				case 0x2C0A: return "Length"; // org.bluetooth.characteristic.length
				case 0x2C0B: return "Torque"; // org.bluetooth.characteristic.torque
				case 0x2C0C: return "IMD Status"; // org.bluetooth.characteristic.imd_status
				case 0x2C0D: return "IMDS Descriptor Value Changed"; // org.bluetooth.characteristic.imds_descriptor_value_changed
				case 0x2C0E: return "First Use Date"; // org.bluetooth.characteristic.first_use_date
				case 0x2C0F: return "Life Cycle Data"; // org.bluetooth.characteristic.life_cycle_data
				case 0x2C10: return "Work Cycle Data"; // org.bluetooth.characteristic.work_cycle_data
				case 0x2C11: return "Service Cycle Data"; // org.bluetooth.characteristic.service_cycle_data
				case 0x2C12: return "IMD Control"; // org.bluetooth.characteristic.imd_control
				case 0x2C13: return "IMD Historical Data"; // org.bluetooth.characteristic.imd_historical_data
				case 0x2C14: return "RAS Features"; // org.bluetooth.characteristic.ras_features
				case 0x2C15: return "Real-time Ranging Data"; // org.bluetooth.characteristic.real-time_ranging_data
				case 0x2C16: return "On-demand Ranging Data"; // org.bluetooth.characteristic.on-demand_ranging_data
				case 0x2C17: return "RAS Control Point"; // org.bluetooth.characteristic.ras_control_point
				case 0x2C18: return "Ranging Data Ready"; // org.bluetooth.characteristic.ranging_data_ready
				case 0x2C19: return "Ranging Data Overwritten"; // org.bluetooth.characteristic.ranging_data_overwritten
				case 0x2C1A: return "Coordinated Set Name"; // org.bluetooth.characteristic.coordinated_set_name
				case 0x2C1B: return "Humidity 8"; // org.bluetooth.characteristic.humidity_8
				case 0x2C1C: return "Illuminance 16"; // org.bluetooth.characteristic.illuminance_16
				case 0x2C1D: return "Acceleration - 3D"; // org.bluetooth.characteristic.acceleration_3d
				case 0x2C1E: return "Precise Acceleration - 3D"; // org.bluetooth.characteristic.precise_acceleration_3d
				case 0x2C1F: return "Acceleration Detection Status"; // org.bluetooth.characteristic.acceleration_detection_status
				case 0x2C20: return "Door/Window Status"; // org.bluetooth.characteristic.door_window_status
				case 0x2C21: return "Pushbutton Status 8"; // org.bluetooth.characteristic.pushbutton_status_8
				case 0x2C22: return "Contact Status 8"; // org.bluetooth.characteristic.contact_status_8
				case 0x2C23: return "HID ISO Properties"; // org.bluetooth.characteristic.hid_iso_properties
				case 0x2C24: return "LE HID Operation Mode"; // org.bluetooth.characteristic.le_hid_operation_mode
				case 0x2C25: return "Cookware Description"; // org.bluetooth.characteristic.cookware_description
				case 0x2C26: return "Recipe Control"; // org.bluetooth.characteristic.recipe_control
				case 0x2C27: return "Recipe Parameters"; // org.bluetooth.characteristic.recipe_parameters
				case 0x2C28: return "Cooking Step Status"; // org.bluetooth.characteristic.cooking_step_status
				case 0x2C29: return "Cooking Zone Capabilities"; // org.bluetooth.characteristic.cooking_zone_capabilities
				case 0x2C2A: return "Cooking Zone Desired Cooking Conditions"; // org.bluetooth.characteristic.cooking_zone_desired_cooking_conditions
				case 0x2C2B: return "Cooking Zone Actual Cooking Conditions"; // org.bluetooth.characteristic.cooking_zone_actual_cooking_conditions
				case 0x2C2C: return "Cookware Sensor Data"; // org.bluetooth.characteristic.cookware_sensor_data
				case 0x2C2D: return "Cookware Sensor Aggregate"; // org.bluetooth.characteristic.cookware_sensor_aggregate
				case 0x2C2E: return "Cooking Temperature"; // org.bluetooth.characteristic.cooking_temperature
				case 0x2C2F: return "Cooking Zone Perceived Power"; // org.bluetooth.characteristic.cooking_zone_preceived_power
				case 0x2C30: return "Kitchen Appliance Airflow"; // org.bluetooth.characteristic.kitchen_appliance_airflow
				case 0x2C31: return "Voice Assistant Name"; // org.bluetooth.characteristic.voice_assistant_name
				case 0x2C32: return "Voice Assistant UUID"; // org.bluetooth.characteristic.voice_assistant_uuid
				case 0x2C33: return "Voice Assistant Service Control Point"; // org.bluetooth.characteristic.voice_assistant_service_control_point
				case 0x2C34: return "Installed Location"; // org.bluetooth.characteristic.installed_location
				case 0x2C35: return "Voice Assistant Session State"; // org.bluetooth.characteristic.voice_assistant_session_state
				case 0x2C36: return "Voice Assistant Session Flag"; // org.bluetooth.characteristic.voice_assistant_session_flag
				case 0x2C37: return "Voice Assistant Supported Languages"; // org.bluetooth.characteristic.voice_assistant_supported_languages
				case 0x2C38: return "Voice Assistant Supported Features"; // org.bluetooth.characteristic.voice_assistant_supported_features
                // endupdatefile:
            }
            return $"{value:X2}";
        }
    }
}
