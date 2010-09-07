﻿using System;
using System.Diagnostics;

namespace HashLib.Crypto
{
    internal class RIPEMD160 : MDBase
    {
        public RIPEMD160()
            : base(5, 20)
        {
        }

        public override void Initialize()
        {
            m_state[0] = 0x67452301;
            m_state[1] = 0xefcdab89;
            m_state[2] = 0x98badcfe;
            m_state[3] = 0x10325476;
            m_state[4] = 0xc3d2e1f0;

            base.Initialize();
        }

        protected override void TransformBlock(byte[] a_data, int a_index)
        {
            uint[] data = new uint[16];
            Converters.ConvertBytesToUInts(a_data, a_index, BlockSize, data);

            uint aa, bb, cc, dd, ee;
            uint a = aa = m_state[0];
            uint b = bb = m_state[1];
            uint c = cc = m_state[2];
            uint d = dd = m_state[3];
            uint e = ee = m_state[4];

            a += data[0] + (b ^ c ^ d);
            a = ((a << 11) | (a >> (32 - 11))) + e;
            c = (c << 10) | (c >> (32 - 10));
            e += data[1] + (a ^ b ^ c);
            e = ((e << 14) | (e >> (32 - 14))) + d;
            b = (b << 10) | (b >> (32 - 10));
            d += data[2] + (e ^ a ^ b);
            d = ((d << 15) | (d >> (32 - 15))) + c;
            a = (a << 10) | (a >> (32 - 10));
            c += data[3] + (d ^ e ^ a);
            c = ((c << 12) | (c >> (32 - 12))) + b;
            e = (e << 10) | (e >> (32 - 10));
            b += data[4] + (c ^ d ^ e);
            b = ((b << 5) | (b >> (32 - 5))) + a;
            d = (d << 10) | (d >> (32 - 10));
            a += data[5] + (b ^ c ^ d);
            a = ((a << 8) | (a >> (32 - 8))) + e;
            c = (c << 10) | (c >> (32 - 10));
            e += data[6] + (a ^ b ^ c);
            e = ((e << 7) | (e >> (32 - 7))) + d;
            b = (b << 10) | (b >> (32 - 10));
            d += data[7] + (e ^ a ^ b);
            d = ((d << 9) | (d >> (32 - 9))) + c;
            a = (a << 10) | (a >> (32 - 10));
            c += data[8] + (d ^ e ^ a);
            c = ((c << 11) | (c >> (32 - 11))) + b;
            e = (e << 10) | (e >> (32 - 10));
            b += data[9] + (c ^ d ^ e);
            b = ((b << 13) | (b >> (32 - 13))) + a;
            d = (d << 10) | (d >> (32 - 10));
            a += data[10] + (b ^ c ^ d);
            a = ((a << 14) | (a >> (32 - 14))) + e;
            c = (c << 10) | (c >> (32 - 10));
            e += data[11] + (a ^ b ^ c);
            e = ((e << 15) | (e >> (32 - 15))) + d;
            b = (b << 10) | (b >> (32 - 10));
            d += data[12] + (e ^ a ^ b);
            d = ((d << 6) | (d >> (32 - 6))) + c;
            a = (a << 10) | (a >> (32 - 10));
            c += data[13] + (d ^ e ^ a);
            c = ((c << 7) | (c >> (32 - 7))) + b;
            e = (e << 10) | (e >> (32 - 10));
            b += data[14] + (c ^ d ^ e);
            b = ((b << 9) | (b >> (32 - 9))) + a;
            d = (d << 10) | (d >> (32 - 10));
            a += data[15] + (b ^ c ^ d);
            a = ((a << 8) | (a >> (32 - 8))) + e;
            c = (c << 10) | (c >> (32 - 10));

            aa += data[5] + C1 + (bb ^ (cc | ~dd));
            aa = ((aa << 8) | (aa >> (32 - 8))) + ee;
            cc = (cc << 10) | (cc >> (32 - 10));
            ee += data[14] + C1 + (aa ^ (bb | ~cc));
            ee = ((ee << 9) | (ee >> (32 - 9))) + dd;
            bb = (bb << 10) | (bb >> (32 - 10));
            dd += data[7] + C1 + (ee ^ (aa | ~bb));
            dd = ((dd << 9) | (dd >> (32 - 9))) + cc;
            aa = (aa << 10) | (aa >> (32 - 10));
            cc += data[0] + C1 + (dd ^ (ee | ~aa));
            cc = ((cc << 11) | (cc >> (32 - 11))) + bb;
            ee = (ee << 10) | (ee >> (32 - 10));
            bb += data[9] + C1 + (cc ^ (dd | ~ee));
            bb = ((bb << 13) | (bb >> (32 - 13))) + aa;
            dd = (dd << 10) | (dd >> (32 - 10));
            aa += data[2] + C1 + (bb ^ (cc | ~dd));
            aa = ((aa << 15) | (aa >> (32 - 15))) + ee;
            cc = (cc << 10) | (cc >> (32 - 10));
            ee += data[11] + C1 + (aa ^ (bb | ~cc));
            ee = ((ee << 15) | (ee >> (32 - 15))) + dd;
            bb = (bb << 10) | (bb >> (32 - 10));
            dd += data[4] + C1 + (ee ^ (aa | ~bb));
            dd = ((dd << 5) | (dd >> (32 - 5))) + cc;
            aa = (aa << 10) | (aa >> (32 - 10));
            cc += data[13] + C1 + (dd ^ (ee | ~aa));
            cc = ((cc << 7) | (cc >> (32 - 7))) + bb;
            ee = (ee << 10) | (ee >> (32 - 10));
            bb += data[6] + C1 + (cc ^ (dd | ~ee));
            bb = ((bb << 7) | (bb >> (32 - 7))) + aa;
            dd = (dd << 10) | (dd >> (32 - 10));
            aa += data[15] + C1 + (bb ^ (cc | ~dd));
            aa = ((aa << 8) | (aa >> (32 - 8))) + ee;
            cc = (cc << 10) | (cc >> (32 - 10));
            ee += data[8] + C1 + (aa ^ (bb | ~cc));
            ee = ((ee << 11) | (ee >> (32 - 11))) + dd;
            bb = (bb << 10) | (bb >> (32 - 10));
            dd += data[1] + C1 + (ee ^ (aa | ~bb));
            dd = ((dd << 14) | (dd >> (32 - 14))) + cc;
            aa = (aa << 10) | (aa >> (32 - 10));
            cc += data[10] + C1 + (dd ^ (ee | ~aa));
            cc = ((cc << 14) | (cc >> (32 - 14))) + bb;
            ee = (ee << 10) | (ee >> (32 - 10));
            bb += data[3] + C1 + (cc ^ (dd | ~ee));
            bb = ((bb << 12) | (bb >> (32 - 12))) + aa;
            dd = (dd << 10) | (dd >> (32 - 10));
            aa += data[12] + C1 + (bb ^ (cc | ~dd));
            aa = ((aa << 6) | (aa >> (32 - 6))) + ee;
            cc = (cc << 10) | (cc >> (32 - 10));

            e += data[7] + C2 + ((a & b) | (~a & c));
            e = ((e << 7) | (e >> (32 - 7))) + d;
            b = (b << 10) | (b >> (32 - 10));
            d += data[4] + C2 + ((e & a) | (~e & b));
            d = ((d << 6) | (d >> (32 - 6))) + c;
            a = (a << 10) | (a >> (32 - 10));
            c += data[13] + C2 + ((d & e) | (~d & a));
            c = ((c << 8) | (c >> (32 - 8))) + b;
            e = (e << 10) | (e >> (32 - 10));
            b += data[1] + C2 + ((c & d) | (~c & e));
            b = ((b << 13) | (b >> (32 - 13))) + a;
            d = (d << 10) | (d >> (32 - 10));
            a += data[10] + C2 + ((b & c) | (~b & d));
            a = ((a << 11) | (a >> (32 - 11))) + e;
            c = (c << 10) | (c >> (32 - 10));
            e += data[6] + C2 + ((a & b) | (~a & c));
            e = ((e << 9) | (e >> (32 - 9))) + d;
            b = (b << 10) | (b >> (32 - 10));
            d += data[15] + C2 + ((e & a) | (~e & b));
            d = ((d << 7) | (d >> (32 - 7))) + c;
            a = (a << 10) | (a >> (32 - 10));
            c += data[3] + C2 + ((d & e) | (~d & a));
            c = ((c << 15) | (c >> (32 - 15))) + b;
            e = (e << 10) | (e >> (32 - 10));
            b += data[12] + C2 + ((c & d) | (~c & e));
            b = ((b << 7) | (b >> (32 - 7))) + a;
            d = (d << 10) | (d >> (32 - 10));
            a += data[0] + C2 + ((b & c) | (~b & d));
            a = ((a << 12) | (a >> (32 - 12))) + e;
            c = (c << 10) | (c >> (32 - 10));
            e += data[9] + C2 + ((a & b) | (~a & c));
            e = ((e << 15) | (e >> (32 - 15))) + d;
            b = (b << 10) | (b >> (32 - 10));
            d += data[5] + C2 + ((e & a) | (~e & b));
            d = ((d << 9) | (d >> (32 - 9))) + c;
            a = (a << 10) | (a >> (32 - 10));
            c += data[2] + C2 + ((d & e) | (~d & a));
            c = ((c << 11) | (c >> (32 - 11))) + b;
            e = (e << 10) | (e >> (32 - 10));
            b += data[14] + C2 + ((c & d) | (~c & e));
            b = ((b << 7) | (b >> (32 - 7))) + a;
            d = (d << 10) | (d >> (32 - 10));
            a += data[11] + C2 + ((b & c) | (~b & d));
            a = ((a << 13) | (a >> (32 - 13))) + e;
            c = (c << 10) | (c >> (32 - 10));
            e += data[8] + C2 + ((a & b) | (~a & c));
            e = ((e << 12) | (e >> (32 - 12))) + d;
            b = (b << 10) | (b >> (32 - 10));

            ee += data[6] + C3 + ((aa & cc) | (bb & ~cc));
            ee = ((ee << 9) | (ee >> (32 - 9))) + dd;
            bb = (bb << 10) | (bb >> (32 - 10));
            dd += data[11] + C3 + ((ee & bb) | (aa & ~bb));
            dd = ((dd << 13) | (dd >> (32 - 13))) + cc;
            aa = (aa << 10) | (aa >> (32 - 10));
            cc += data[3] + C3 + ((dd & aa) | (ee & ~aa));
            cc = ((cc << 15) | (cc >> (32 - 15))) + bb;
            ee = (ee << 10) | (ee >> (32 - 10));
            bb += data[7] + C3 + ((cc & ee) | (dd & ~ee));
            bb = ((bb << 7) | (bb >> (32 - 7))) + aa;
            dd = (dd << 10) | (dd >> (32 - 10));
            aa += data[0] + C3 + ((bb & dd) | (cc & ~dd));
            aa = ((aa << 12) | (aa >> (32 - 12))) + ee;
            cc = (cc << 10) | (cc >> (32 - 10));
            ee += data[13] + C3 + ((aa & cc) | (bb & ~cc));
            ee = ((ee << 8) | (ee >> (32 - 8))) + dd;
            bb = (bb << 10) | (bb >> (32 - 10));
            dd += data[5] + C3 + ((ee & bb) | (aa & ~bb));
            dd = ((dd << 9) | (dd >> (32 - 9))) + cc;
            aa = (aa << 10) | (aa >> (32 - 10));
            cc += data[10] + C3 + ((dd & aa) | (ee & ~aa));
            cc = ((cc << 11) | (cc >> (32 - 11))) + bb;
            ee = (ee << 10) | (ee >> (32 - 10));
            bb += data[14] + C3 + ((cc & ee) | (dd & ~ee));
            bb = ((bb << 7) | (bb >> (32 - 7))) + aa;
            dd = (dd << 10) | (dd >> (32 - 10));
            aa += data[15] + C3 + ((bb & dd) | (cc & ~dd));
            aa = ((aa << 7) | (aa >> (32 - 7))) + ee;
            cc = (cc << 10) | (cc >> (32 - 10));
            ee += data[8] + C3 + ((aa & cc) | (bb & ~cc));
            ee = ((ee << 12) | (ee >> (32 - 12))) + dd;
            bb = (bb << 10) | (bb >> (32 - 10));
            dd += data[12] + C3 + ((ee & bb) | (aa & ~bb));
            dd = ((dd << 7) | (dd >> (32 - 7))) + cc;
            aa = (aa << 10) | (aa >> (32 - 10));
            cc += data[4] + C3 + ((dd & aa) | (ee & ~aa));
            cc = ((cc << 6) | (cc >> (32 - 6))) + bb;
            ee = (ee << 10) | (ee >> (32 - 10));
            bb += data[9] + C3 + ((cc & ee) | (dd & ~ee));
            bb = ((bb << 15) | (bb >> (32 - 15))) + aa;
            dd = (dd << 10) | (dd >> (32 - 10));
            aa += data[1] + C3 + ((bb & dd) | (cc & ~dd));
            aa = ((aa << 13) | (aa >> (32 - 13))) + ee;
            cc = (cc << 10) | (cc >> (32 - 10));
            ee += data[2] + C3 + ((aa & cc) | (bb & ~cc));
            ee = ((ee << 11) | (ee >> (32 - 11))) + dd;
            bb = (bb << 10) | (bb >> (32 - 10));

            d += data[3] + C4 + ((e | ~a) ^ b);
            d = ((d << 11) | (d >> (32 - 11))) + c;
            a = (a << 10) | (a >> (32 - 10));
            c += data[10] + C4 + ((d | ~e) ^ a);
            c = ((c << 13) | (c >> (32 - 13))) + b;
            e = (e << 10) | (e >> (32 - 10));
            b += data[14] + C4 + ((c | ~d) ^ e);
            b = ((b << 6) | (b >> (32 - 6))) + a;
            d = (d << 10) | (d >> (32 - 10));
            a += data[4] + C4 + ((b | ~c) ^ d);
            a = ((a << 7) | (a >> (32 - 7))) + e;
            c = (c << 10) | (c >> (32 - 10));
            e += data[9] + C4 + ((a | ~b) ^ c);
            e = ((e << 14) | (e >> (32 - 14))) + d;
            b = (b << 10) | (b >> (32 - 10));
            d += data[15] + C4 + ((e | ~a) ^ b);
            d = ((d << 9) | (d >> (32 - 9))) + c;
            a = (a << 10) | (a >> (32 - 10));
            c += data[8] + C4 + ((d | ~e) ^ a);
            c = ((c << 13) | (c >> (32 - 13))) + b;
            e = (e << 10) | (e >> (32 - 10));
            b += data[1] + C4 + ((c | ~d) ^ e);
            b = ((b << 15) | (b >> (32 - 15))) + a;
            d = (d << 10) | (d >> (32 - 10));
            a += data[2] + C4 + ((b | ~c) ^ d);
            a = ((a << 14) | (a >> (32 - 14))) + e;
            c = (c << 10) | (c >> (32 - 10));
            e += data[7] + C4 + ((a | ~b) ^ c);
            e = ((e << 8) | (e >> (32 - 8))) + d;
            b = (b << 10) | (b >> (32 - 10));
            d += data[0] + C4 + ((e | ~a) ^ b);
            d = ((d << 13) | (d >> (32 - 13))) + c;
            a = (a << 10) | (a >> (32 - 10));
            c += data[6] + C4 + ((d | ~e) ^ a);
            c = ((c << 6) | (c >> (32 - 6))) + b;
            e = (e << 10) | (e >> (32 - 10));
            b += data[13] + C4 + ((c | ~d) ^ e);
            b = ((b << 5) | (b >> (32 - 5))) + a;
            d = (d << 10) | (d >> (32 - 10));
            a += data[11] + C4 + ((b | ~c) ^ d);
            a = ((a << 12) | (a >> (32 - 12))) + e;
            c = (c << 10) | (c >> (32 - 10));
            e += data[5] + C4 + ((a | ~b) ^ c);
            e = ((e << 7) | (e >> (32 - 7))) + d;
            b = (b << 10) | (b >> (32 - 10));
            d += data[12] + C4 + ((e | ~a) ^ b);
            d = ((d << 5) | (d >> (32 - 5))) + c;
            a = (a << 10) | (a >> (32 - 10));

            dd += data[15] + C5 + ((ee | ~aa) ^ bb);
            dd = ((dd << 9) | (dd >> (32 - 9))) + cc;
            aa = (aa << 10) | (aa >> (32 - 10));
            cc += data[5] + C5 + ((dd | ~ee) ^ aa);
            cc = ((cc << 7) | (cc >> (32 - 7))) + bb;
            ee = (ee << 10) | (ee >> (32 - 10));
            bb += data[1] + C5 + ((cc | ~dd) ^ ee);
            bb = ((bb << 15) | (bb >> (32 - 15))) + aa;
            dd = (dd << 10) | (dd >> (32 - 10));
            aa += data[3] + C5 + ((bb | ~cc) ^ dd);
            aa = ((aa << 11) | (aa >> (32 - 11))) + ee;
            cc = (cc << 10) | (cc >> (32 - 10));
            ee += data[7] + C5 + ((aa | ~bb) ^ cc);
            ee = ((ee << 8) | (ee >> (32 - 8))) + dd;
            bb = (bb << 10) | (bb >> (32 - 10));
            dd += data[14] + C5 + ((ee | ~aa) ^ bb);
            dd = ((dd << 6) | (dd >> (32 - 6))) + cc;
            aa = (aa << 10) | (aa >> (32 - 10));
            cc += data[6] + C5 + ((dd | ~ee) ^ aa);
            cc = ((cc << 6) | (cc >> (32 - 6))) + bb;
            ee = (ee << 10) | (ee >> (32 - 10));
            bb += data[9] + C5 + ((cc | ~dd) ^ ee);
            bb = ((bb << 14) | (bb >> (32 - 14))) + aa;
            dd = (dd << 10) | (dd >> (32 - 10));
            aa += data[11] + C5 + ((bb | ~cc) ^ dd);
            aa = ((aa << 12) | (aa >> (32 - 12))) + ee;
            cc = (cc << 10) | (cc >> (32 - 10));
            ee += data[8] + C5 + ((aa | ~bb) ^ cc);
            ee = ((ee << 13) | (ee >> (32 - 13))) + dd;
            bb = (bb << 10) | (bb >> (32 - 10));
            dd += data[12] + C5 + ((ee | ~aa) ^ bb);
            dd = ((dd << 5) | (dd >> (32 - 5))) + cc;
            aa = (aa << 10) | (aa >> (32 - 10));
            cc += data[2] + C5 + ((dd | ~ee) ^ aa);
            cc = ((cc << 14) | (cc >> (32 - 14))) + bb;
            ee = (ee << 10) | (ee >> (32 - 10));
            bb += data[10] + C5 + ((cc | ~dd) ^ ee);
            bb = ((bb << 13) | (bb >> (32 - 13))) + aa;
            dd = (dd << 10) | (dd >> (32 - 10));
            aa += data[0] + C5 + ((bb | ~cc) ^ dd);
            aa = ((aa << 13) | (aa >> (32 - 13))) + ee;
            cc = (cc << 10) | (cc >> (32 - 10));
            ee += data[4] + C5 + ((aa | ~bb) ^ cc);
            ee = ((ee << 7) | (ee >> (32 - 7))) + dd;
            bb = (bb << 10) | (bb >> (32 - 10));
            dd += data[13] + C5 + ((ee | ~aa) ^ bb);
            dd = ((dd << 5) | (dd >> (32 - 5))) + cc;
            aa = (aa << 10) | (aa >> (32 - 10));

            c += data[1] + C6 + ((d & a) | (e & ~a));
            c = ((c << 11) | (c >> (32 - 11))) + b;
            e = (e << 10) | (e >> (32 - 10));
            b += data[9] + C6 + ((c & e) | (d & ~e));
            b = ((b << 12) | (b >> (32 - 12))) + a;
            d = (d << 10) | (d >> (32 - 10));
            a += data[11] + C6 + ((b & d) | (c & ~d));
            a = ((a << 14) | (a >> (32 - 14))) + e;
            c = (c << 10) | (c >> (32 - 10));
            e += data[10] + C6 + ((a & c) | (b & ~c));
            e = ((e << 15) | (e >> (32 - 15))) + d;
            b = (b << 10) | (b >> (32 - 10));
            d += data[0] + C6 + ((e & b) | (a & ~b));
            d = ((d << 14) | (d >> (32 - 14))) + c;
            a = (a << 10) | (a >> (32 - 10));
            c += data[8] + C6 + ((d & a) | (e & ~a));
            c = ((c << 15) | (c >> (32 - 15))) + b;
            e = (e << 10) | (e >> (32 - 10));
            b += data[12] + C6 + ((c & e) | (d & ~e));
            b = ((b << 9) | (b >> (32 - 9))) + a;
            d = (d << 10) | (d >> (32 - 10));
            a += data[4] + C6 + ((b & d) | (c & ~d));
            a = ((a << 8) | (a >> (32 - 8))) + e;
            c = (c << 10) | (c >> (32 - 10));
            e += data[13] + C6 + ((a & c) | (b & ~c));
            e = ((e << 9) | (e >> (32 - 9))) + d;
            b = (b << 10) | (b >> (32 - 10));
            d += data[3] + C6 + ((e & b) | (a & ~b));
            d = ((d << 14) | (d >> (32 - 14))) + c;
            a = (a << 10) | (a >> (32 - 10));
            c += data[7] + C6 + ((d & a) | (e & ~a));
            c = ((c << 5) | (c >> (32 - 5))) + b;
            e = (e << 10) | (e >> (32 - 10));
            b += data[15] + C6 + ((c & e) | (d & ~e));
            b = ((b << 6) | (b >> (32 - 6))) + a;
            d = (d << 10) | (d >> (32 - 10));
            a += data[14] + C6 + ((b & d) | (c & ~d));
            a = ((a << 8) | (a >> (32 - 8))) + e;
            c = (c << 10) | (c >> (32 - 10));
            e += data[5] + C6 + ((a & c) | (b & ~c));
            e = ((e << 6) | (e >> (32 - 6))) + d;
            b = (b << 10) | (b >> (32 - 10));
            d += data[6] + C6 + ((e & b) | (a & ~b));
            d = ((d << 5) | (d >> (32 - 5))) + c;
            a = (a << 10) | (a >> (32 - 10));
            c += data[2] + C6 + ((d & a) | (e & ~a));
            c = ((c << 12) | (c >> (32 - 12))) + b;
            e = (e << 10) | (e >> (32 - 10));

            cc += data[8] + C7 + ((dd & ee) | (~dd & aa));
            cc = ((cc << 15) | (cc >> (32 - 15))) + bb;
            ee = (ee << 10) | (ee >> (32 - 10));
            bb += data[6] + C7 + ((cc & dd) | (~cc & ee));
            bb = ((bb << 5) | (bb >> (32 - 5))) + aa;
            dd = (dd << 10) | (dd >> (32 - 10));
            aa += data[4] + C7 + ((bb & cc) | (~bb & dd));
            aa = ((aa << 8) | (aa >> (32 - 8))) + ee;
            cc = (cc << 10) | (cc >> (32 - 10));
            ee += data[1] + C7 + ((aa & bb) | (~aa & cc));
            ee = ((ee << 11) | (ee >> (32 - 11))) + dd;
            bb = (bb << 10) | (bb >> (32 - 10));
            dd += data[3] + C7 + ((ee & aa) | (~ee & bb));
            dd = ((dd << 14) | (dd >> (32 - 14))) + cc;
            aa = (aa << 10) | (aa >> (32 - 10));
            cc += data[11] + C7 + ((dd & ee) | (~dd & aa));
            cc = ((cc << 14) | (cc >> (32 - 14))) + bb;
            ee = (ee << 10) | (ee >> (32 - 10));
            bb += data[15] + C7 + ((cc & dd) | (~cc & ee));
            bb = ((bb << 6) | (bb >> (32 - 6))) + aa;
            dd = (dd << 10) | (dd >> (32 - 10));
            aa += data[0] + C7 + ((bb & cc) | (~bb & dd));
            aa = ((aa << 14) | (aa >> (32 - 14))) + ee;
            cc = (cc << 10) | (cc >> (32 - 10));
            ee += data[5] + C7 + ((aa & bb) | (~aa & cc));
            ee = ((ee << 6) | (ee >> (32 - 6))) + dd;
            bb = (bb << 10) | (bb >> (32 - 10));
            dd += data[12] + C7 + ((ee & aa) | (~ee & bb));
            dd = ((dd << 9) | (dd >> (32 - 9))) + cc;
            aa = (aa << 10) | (aa >> (32 - 10));
            cc += data[2] + C7 + ((dd & ee) | (~dd & aa));
            cc = ((cc << 12) | (cc >> (32 - 12))) + bb;
            ee = (ee << 10) | (ee >> (32 - 10));
            bb += data[13] + C7 + ((cc & dd) | (~cc & ee));
            bb = ((bb << 9) | (bb >> (32 - 9))) + aa;
            dd = (dd << 10) | (dd >> (32 - 10));
            aa += data[9] + C7 + ((bb & cc) | (~bb & dd));
            aa = ((aa << 12) | (aa >> (32 - 12))) + ee;
            cc = (cc << 10) | (cc >> (32 - 10));
            ee += data[7] + C7 + ((aa & bb) | (~aa & cc));
            ee = ((ee << 5) | (ee >> (32 - 5))) + dd;
            bb = (bb << 10) | (bb >> (32 - 10));
            dd += data[10] + C7 + ((ee & aa) | (~ee & bb));
            dd = ((dd << 15) | (dd >> (32 - 15))) + cc;
            aa = (aa << 10) | (aa >> (32 - 10));
            cc += data[14] + C7 + ((dd & ee) | (~dd & aa));
            cc = ((cc << 8) | (cc >> (32 - 8))) + bb;
            ee = (ee << 10) | (ee >> (32 - 10));

            b += data[4] + C8 + (c ^ (d | ~e));
            b = ((b << 9) | (b >> (32 - 9))) + a;
            d = (d << 10) | (d >> (32 - 10));
            a += data[0] + C8 + (b ^ (c | ~d));
            a = ((a << 15) | (a >> (32 - 15))) + e;
            c = (c << 10) | (c >> (32 - 10));
            e += data[5] + C8 + (a ^ (b | ~c));
            e = ((e << 5) | (e >> (32 - 5))) + d;
            b = (b << 10) | (b >> (32 - 10));
            d += data[9] + C8 + (e ^ (a | ~b));
            d = ((d << 11) | (d >> (32 - 11))) + c;
            a = (a << 10) | (a >> (32 - 10));
            c += data[7] + C8 + (d ^ (e | ~a));
            c = ((c << 6) | (c >> (32 - 6))) + b;
            e = (e << 10) | (e >> (32 - 10));
            b += data[12] + C8 + (c ^ (d | ~e));
            b = ((b << 8) | (b >> (32 - 8))) + a;
            d = (d << 10) | (d >> (32 - 10));
            a += data[2] + C8 + (b ^ (c | ~d));
            a = ((a << 13) | (a >> (32 - 13))) + e;
            c = (c << 10) | (c >> (32 - 10));
            e += data[10] + C8 + (a ^ (b | ~c));
            e = ((e << 12) | (e >> (32 - 12))) + d;
            b = (b << 10) | (b >> (32 - 10));
            d += data[14] + C8 + (e ^ (a | ~b));
            d = ((d << 5) | (d >> (32 - 5))) + c;
            a = (a << 10) | (a >> (32 - 10));
            c += data[1] + C8 + (d ^ (e | ~a));
            c = ((c << 12) | (c >> (32 - 12))) + b;
            e = (e << 10) | (e >> (32 - 10));
            b += data[3] + C8 + (c ^ (d | ~e));
            b = ((b << 13) | (b >> (32 - 13))) + a;
            d = (d << 10) | (d >> (32 - 10));
            a += data[8] + C8 + (b ^ (c | ~d));
            a = ((a << 14) | (a >> (32 - 14))) + e;
            c = (c << 10) | (c >> (32 - 10));
            e += data[11] + C8 + (a ^ (b | ~c));
            e = ((e << 11) | (e >> (32 - 11))) + d;
            b = (b << 10) | (b >> (32 - 10));
            d += data[6] + C8 + (e ^ (a | ~b));
            d = ((d << 8) | (d >> (32 - 8))) + c;
            a = (a << 10) | (a >> (32 - 10));
            c += data[15] + C8 + (d ^ (e | ~a));
            c = ((c << 5) | (c >> (32 - 5))) + b;
            e = (e << 10) | (e >> (32 - 10));
            b += data[13] + C8 + (c ^ (d | ~e));
            b = ((b << 6) | (b >> (32 - 6))) + a;
            d = (d << 10) | (d >> (32 - 10));

            bb += data[12] + (cc ^ dd ^ ee);
            bb = ((bb << 8) | (bb >> (32 - 8))) + aa;
            dd = (dd << 10) | (dd >> (32 - 10));
            aa += data[15] + (bb ^ cc ^ dd);
            aa = ((aa << 5) | (aa >> (32 - 5))) + ee;
            cc = (cc << 10) | (cc >> (32 - 10));
            ee += data[10] + (aa ^ bb ^ cc);
            ee = ((ee << 12) | (ee >> (32 - 12))) + dd;
            bb = (bb << 10) | (bb >> (32 - 10));
            dd += data[4] + (ee ^ aa ^ bb);
            dd = ((dd << 9) | (dd >> (32 - 9))) + cc;
            aa = (aa << 10) | (aa >> (32 - 10));
            cc += data[1] + (dd ^ ee ^ aa);
            cc = ((cc << 12) | (cc >> (32 - 12))) + bb;
            ee = (ee << 10) | (ee >> (32 - 10));
            bb += data[5] + (cc ^ dd ^ ee);
            bb = ((bb << 5) | (bb >> (32 - 5))) + aa;
            dd = (dd << 10) | (dd >> (32 - 10));
            aa += data[8] + (bb ^ cc ^ dd);
            aa = ((aa << 14) | (aa >> (32 - 14))) + ee;
            cc = (cc << 10) | (cc >> (32 - 10));
            ee += data[7] + (aa ^ bb ^ cc);
            ee = ((ee << 6) | (ee >> (32 - 6))) + dd;
            bb = (bb << 10) | (bb >> (32 - 10));
            dd += data[6] + (ee ^ aa ^ bb);
            dd = ((dd << 8) | (dd >> (32 - 8))) + cc;
            aa = (aa << 10) | (aa >> (32 - 10));
            cc += data[2] + (dd ^ ee ^ aa);
            cc = ((cc << 13) | (cc >> (32 - 13))) + bb;
            ee = (ee << 10) | (ee >> (32 - 10));
            bb += data[13] + (cc ^ dd ^ ee);
            bb = ((bb << 6) | (bb >> (32 - 6))) + aa;
            dd = (dd << 10) | (dd >> (32 - 10));
            aa += data[14] + (bb ^ cc ^ dd);
            aa = ((aa << 5) | (aa >> (32 - 5))) + ee;
            cc = (cc << 10) | (cc >> (32 - 10));
            ee += data[0] + (aa ^ bb ^ cc);
            ee = ((ee << 15) | (ee >> (32 - 15))) + dd;
            bb = (bb << 10) | (bb >> (32 - 10));
            dd += data[3] + (ee ^ aa ^ bb);
            dd = ((dd << 13) | (dd >> (32 - 13))) + cc;
            aa = (aa << 10) | (aa >> (32 - 10));
            cc += data[9] + (dd ^ ee ^ aa);
            cc = ((cc << 11) | (cc >> (32 - 11))) + bb;
            ee = (ee << 10) | (ee >> (32 - 10));
            bb += data[11] + (cc ^ dd ^ ee);
            bb = ((bb << 11) | (bb >> (32 - 11))) + aa;
            dd = (dd << 10) | (dd >> (32 - 10));

            dd += c + m_state[1];
            m_state[1] = m_state[2] + d + ee;
            m_state[2] = m_state[3] + e + aa;
            m_state[3] = m_state[4] + a + bb;
            m_state[4] = m_state[0] + b + cc;
            m_state[0] = dd;
        }
    }
}
