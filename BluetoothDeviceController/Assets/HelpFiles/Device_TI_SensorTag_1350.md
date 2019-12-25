# TI SensorTag CC1350 and CC2650

![SensorTag 1350](../DevicePictures/TI_SensorTag_1350-175.png)

These are the two latest TI SensorTags. The SensorTag is a small device
created by TI in order to demonstrate the cababilities of their microprocessors when 
dealing with a variety of on-board sensors. 

# Example

![Accelerometer](../ScreenShots/Device_TI_SensorTag_1350-175.png)

# Pairing (code 0) and usage details
The device does not need to be paired with Windows before it can be used.

![SensorTag 1350](../ScreenShots/Device_TI_SensorTag_1350_Humidity.png)

The SensorTag can be a little finicky to connect; sometimes you have to try several times, 
pressing the pairing button. 

If you are creating your own program to read SensorTag data, you have to do a two-step setup to 
get data. You have to both enable the sensors with the configure option and must also enable 
the Bluetooth notifications. This is done for you automatically in the program.


# Sensor Details
The device inludes the following sensors. Later versions of the SensorTag **removed** the IR temperature sensor
and updated the magnet sensor.

* [**TMP007** IR Temperature Sensor](http://www.ti.com/product/tmp007) from Texas Instruments
* [**MPU-9250** Motion Sensor](https://www.invensense.com/products/motion-tracking/9-axis/mpu-9250/) from InvenSense
* [**HDC-1000** Humidity Sensor](http://www.ti.com/product/HDC1000) from TI
* [**BMP280** Pressure Sensor](https://www.bosch-sensortec.com/bst/products/all_products/bmp280) from Bosch
* [**OPT3001** Ambient Light Sensor](http://www.ti.com/product/OPT3001) from TI


