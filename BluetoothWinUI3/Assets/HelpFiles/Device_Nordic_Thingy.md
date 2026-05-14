# Nordic Thingy:52

The Nordic Thingy:52 is an easy-to use rechargable environmental sensor. It's designed to help engineers work with the Nordic family of parts; it's not commonly used by ordinary people.

![Nordic Thingy:52](../DevicePictures/Nordic_Thingy_ScreenShot_01.png&size_200x200)

## Getting started

The sensor does not need to be paired before using. The app will automatically detect the sensor from its advertisements and will connect automatically without any intervension, and the environment data will start to be reported.

The temperature, pressure, and humidity will be displayed in less than 10 seconds. The advanced sensor data (eCOS and TVOC values) can take up to a minute to be reported, and the first reports are often incorrect.

## eCOS and TVOC

Volitile Organic Compunds (VOC) measures the amounts of alcohols, ketones, amines and more in the atmosphere. The output measurement is in parts per billion.

Equivilent CO₂ is a calculated amount of CO₂. The measurement range is in parts per million, and a normal reading is between 400 and 500. The reading will always start off as **390**; this is the indication that the sensor has not yet initialized.

"AMS recommends that you run this sensor for 48 hours when you first receive it to "burn it in", and then 20 minutes in the desired mode every time the sensor is in use. This is because the sensitivity levels of the sensor will change during early use."

## DeviceDetails

The Nordic Thingy:52 (part number nRF6936) demonstrates the use of the Nordic Semiconductor nRF52832. It includes 

* a LPS22HB Pressure/Altitude and Temperature sensor
* a HTS221 Humidity and Temperature sensor
* a BH1745NUC color sensor
* a CCS811 TVOC and eCOS sensor from AMS

## Links

* [Nordic Thingy:52](https://www.nordicsemi.com/Products/Development-hardware/Nordic-Thingy-52)
* [Product Brief PDF](https://nsscprodmedia.blob.core.windows.net/prod/software-and-other-downloads/product-briefs/nordic-thingy52-product-brief.pdf)
* [Technical Details PDF](https://docs-be.nordicsemi.com/bundle/ug_thingy52/attach/Thingy52_UG_v1.2.pdf?_LANG=enus)
* [CCS811 sensor at AdaFruit](https://learn.adafruit.com/adafruit-ccs811-air-quality-sensor/overview)
* [AMS Press release for CCS811](https://ams-osram.com/news/press-releases/ams-ccs8xx-product-family-of-voc-sensors-enhances-end-user-experience-for-indoor-air-quality-monitoring)
* [CCS811 data sheet](https://media.ic-find.com/datasheets/d15552093f00d1142ede2fc5c444b2a4.pdf)