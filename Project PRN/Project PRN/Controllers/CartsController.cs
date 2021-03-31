﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Project_PRN.Models;

namespace Project_PRN.Controllers
{
    public class CartsController : Controller
    {
        private ProjectPRNEntities3 db = new ProjectPRNEntities3();


        public ActionResult Cart()
        {
            return View();
        }

        public JsonResult CartJson()
        {//Chọn bảng cart qua userID, từ kết quả chọn ra ID của product load lên data
            try
            {
                if (Session["user"] != null)//da login
                {
                    int userId = Int32.Parse(Session["user"].ToString());
                    //Chọn tất cả cart của ng dùng đã log in
                    db.Configuration.ProxyCreationEnabled = false;
                    List<Cart> listCart = db.Carts.Where(c => c.userid == userId).ToList().Select(Cart => new Cart
                    {
                        //Hung: toi them gia tri cartid giup truy van cho phan RemoveProductAtCart()
                        cartid = Cart.cartid,
                        userid = userId,
                        quantity = Cart.quantity,
                        productid = Cart.productid,
                        Product = db.Products.ToList().Select(product => new Product
                        {
                            productID = product.productID,
                            title = product.title,
                            author = product.author,
                            description = product.description,
                            shortDescription = product.shortDescription,
                            image = product.fullImagePath(),
                            price = product.price,
                            quantity = product.quantity,
                            sold = product.sold,
                            postTime = product.postTime,
                            categoriesID = product.categoriesID,
                            userID = product.userID,
                        }).Where(p => p.productID == Cart.productid).FirstOrDefault()
                    }).ToList();
                    return Json(listCart, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //nếu chưa login. lấy dữ liệu từ cookies
                    var serializer = new JavaScriptSerializer();
                    Dictionary<string, int> cart;
                    if (Request.Cookies["cart"] != null)
                    {
                        string cartJson = Request.Cookies["cart"].Value;
                        cart = serializer.Deserialize<Dictionary<string, int>>(cartJson);
                        Dictionary<string, int>.KeyCollection keys = cart.Keys;
                        List<Cart> carts = new List<Cart>();
                        foreach (string key in keys)
                        {
                            int productid = Int32.Parse(key);
                            int quatity = cart[key];
                            Cart newCart = new Cart()
                            {
                                quantity = quatity,
                                Product = db.Products.ToList().Select(product => new Product
                                {
                                    productID = product.productID,
                                    title = product.title,
                                    author = product.author,
                                    description = product.description,
                                    shortDescription = product.shortDescription,
                                    image = product.fullImagePath(),
                                    price = product.price,
                                    quantity = product.quantity,
                                    sold = product.sold,
                                    postTime = product.postTime,
                                    categoriesID = product.categoriesID,
                                    userID = product.userID,
                                }).Where(p => p.productID == productid).FirstOrDefault()
                            };
                            carts.Add(newCart);
                        }
                        return Json(carts, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch
            {
            }
            return Json(db.Products.ToList(), JsonRequestBehavior.AllowGet);
        }

        //Add item into cart
        public JsonResult AddToCart(int productID, int quantity)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                //userID from session
                //check is user logged in
                if (Session["user"] != null)
                {
                    //logged in case, storage cart in database

                    int userID = Int32.Parse(Session["user"].ToString());

                    //select items from cart with userID and productid
                    Cart cart = db.Carts.Where(c => c.userid == userID).Where(c => c.productid == productID).FirstOrDefault();
                    if (cart == null)
                    {
                        //in null case, add new items to database
                        cart = new Cart();
                        cart.userid = userID;
                        cart.productid = productID;
                        cart.quantity = quantity;
                        db.Carts.Add(cart);
                        db.SaveChanges();
                    }
                    else
                    {
                        //in exsisted case, change quantity
                        cart.quantity += quantity;

                        db.Entry(cart).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else
                {
                    //didn't log in case, storage cart in cookies
                    var serializer = new JavaScriptSerializer();

                    Dictionary<string, int> cart;

                    //check is exsisted cart in cookies
                    if (Request.Cookies["cart"] != null)
                    {
                        //exsisted case, pick up it
                        string cartJson = Request.Cookies["cart"].Value;
                        cart = serializer.Deserialize<Dictionary<string, int>>(cartJson);
                    }
                    else
                    {
                        //not exsisted case, declare new cart
                        cart = new Dictionary<string, int>();
                    }

                    //check is exsisted item in cart
                    if (cart.ContainsKey(productID.ToString()))
                    {
                        //in exsisted case, change quantity
                        int currentQuantity = cart[productID.ToString()];
                        cart[productID.ToString()] = currentQuantity + quantity;
                    }
                    else
                    {
                        //in not exsisted case, add new item to cart
                        cart[productID.ToString()] = quantity;
                    }

                    //save into cookies
                    string cartValue = serializer.Serialize(cart);
                    Response.Cookies["cart"].Value = cartValue;
                    Response.Cookies["cart"].Expires = DateTime.Now.AddDays(30);

                }
                return Json(new
                {
                    message = "Product Added Successfully!",
                    type = 1
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(
                    new
                    {
                        message = "Product Added Fail!",
                        type = 2
                    }, JsonRequestBehavior.AllowGet);
            }
        }



        public void AddToCartWhenLogin(string cartJson, int userID)
        {
            var serializer = new JavaScriptSerializer();
            Dictionary<string, int> cookieCart = new Dictionary<string, int>();
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                cookieCart = serializer.Deserialize<Dictionary<string, int>>(cartJson);

                Dictionary<string, int>.KeyCollection key = cookieCart.Keys;
                foreach (string k in key)
                {
                    int productId = Int32.Parse(k);
                    Cart cart = db.Carts.Where(c => c.userid == userID).Where(c => c.productid == productId).FirstOrDefault();
                    if (cart == null)
                    {
                        Cart newCart = new Cart();
                        newCart.userid = userID;
                        newCart.productid = productId;
                        newCart.quantity = cookieCart[k];
                        db.Carts.Add(newCart);
                        db.SaveChanges();
                    }
                    else
                    {
                        cart.quantity += cookieCart[k];
                        db.Entry(cart).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch
            {

            }
        }


        public JsonResult RemoveProductAtCart(int cartId, int productID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            if (Session["user"] != null)
            {
                // Tim Cart theo cartId roi remove
                db.Carts.Remove(db.Carts.Find(cartId));
                db.SaveChanges();
                JsonResult cartUpdated = CartJson();
                return cartUpdated;
            }
            else
            {
                var serializer = new JavaScriptSerializer();
                Dictionary<string, int> cart;
                //
                string cartJson = Request.Cookies["cart"].Value;
                cart = serializer.Deserialize<Dictionary<string, int>>(cartJson);
                cart.Remove(productID.ToString());
                string cartValue = serializer.Serialize(cart);
                Response.Cookies["cart"].Value = cartValue;
                JsonResult cartUpdated = CartJson();
                return cartUpdated;
            }
            return CartJson();
        }

        public JsonResult ChangeQuantityAtCart(int cartId, int productID, int quantity)
        {
            db.Configuration.ProxyCreationEnabled = false;
            if (Session["user"] != null)
            {
                // Get Cart bang cartId
                Cart cart = db.Carts.Find(cartId);
                // Kiem tra quantity cua Product sau khi thay doi so luong la bao nhieu
                int amount = cart.quantity + quantity;
                // Neu nho hon 0 thi khong cho giam nua
                if (amount < 1)
                {
                    JsonResult cartUpdated = CartJson();
                    return cartUpdated;
                }
                else if (amount >= 1) // Neu lon hon 0 thi tien hanh thay doi quantity
                {
                    AddToCart(productID, quantity);
                    JsonResult cartUpdated = CartJson();
                    return cartUpdated;
                }
            } else
            {
                var serializer = new JavaScriptSerializer();
                Dictionary<string, int> cart;
                //
                string cartJson = Request.Cookies["cart"].Value;
                cart = serializer.Deserialize<Dictionary<string, int>>(cartJson);
                int amount = cart[productID.ToString()] + quantity;
                if (amount < 1)
                {
                    JsonResult cartUpdated = CartJson();
                    return cartUpdated;
                
                }
                else if (amount >= 1) // Neu lon hon 0 thi tien hanh thay doi quantity
                {
                    int currentQuantity = cart[productID.ToString()];
                    cart[productID.ToString()] = currentQuantity + quantity;
                    string cartValue = serializer.Serialize(cart);
                    Response.Cookies["cart"].Value = cartValue;
                    JsonResult cartUpdated = CartJson();
                    return cartUpdated;
                }

            }

            return CartJson();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
