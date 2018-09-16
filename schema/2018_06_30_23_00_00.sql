--
-- PostgreSQL database dump
--

-- Dumped from database version 9.6.8
-- Dumped by pg_dump version 9.6.8

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


--
-- Name: myfunc(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.myfunc() RETURNS void
    LANGUAGE plpgsql
    AS $$
  declare
    pfid int;
  begin
    select fif_id from fin_field t where t.ident = 'FACEUNIT' into pfid;
    insert into dict_item(fif_id, key_v, val, ord_id)
	values
	(pfid, 'CHF', 'Швейцарский франк', 1),
	(pfid, 'RUB', 'Российский рубль', 2),
	(pfid, 'USD', 'Доллар США ', 3),
	(pfid, 'EUR', 'Евро', 4),
	(pfid, 'GBP', 'Британский фунт', 5);
    raise notice '%', pfid;
  end $$;


ALTER FUNCTION public.myfunc() OWNER TO postgres;

--
-- Name: upper_foo_on_insert(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.upper_foo_on_insert() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
    BEGIN        
        NEW.ident = UPPER(NEW.ident);
        RETURN NEW;
    END;
$$;


ALTER FUNCTION public.upper_foo_on_insert() OWNER TO postgres;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: calendar; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.calendar (
    cal_id integer NOT NULL,
    ident character varying(100) NOT NULL,
    title character varying(100),
    description character varying(1025)
);


ALTER TABLE public.calendar OWNER TO postgres;

--
-- Name: cashflow; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cashflow (
    cf_id integer NOT NULL,
    val numeric,
    dat date NOT NULL,
    valid_dat date NOT NULL
);


ALTER TABLE public.cashflow OWNER TO postgres;

--
-- Name: cashflow_types; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cashflow_types (
    ct_id integer NOT NULL,
    ident character varying(100) NOT NULL,
    title character varying(100) NOT NULL,
    description character varying(100) NOT NULL
);


ALTER TABLE public.cashflow_types OWNER TO postgres;

--
-- Name: curve_list; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.curve_list (
    cur_id integer NOT NULL,
    fi_id integer NOT NULL,
    term numeric
);


ALTER TABLE public.curve_list OWNER TO postgres;

--
-- Name: curves; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.curves (
    cur_id integer NOT NULL,
    ident character varying(100) NOT NULL,
    description character varying(1000),
    title character varying(100) NOT NULL
);


ALTER TABLE public.curves OWNER TO postgres;

--
-- Name: data_source; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.data_source (
    ds_id integer NOT NULL,
    ident character varying(100) NOT NULL,
    description character varying(1000) DEFAULT NULL::character varying,
    ord_id integer
);


ALTER TABLE public.data_source OWNER TO postgres;

--
-- Name: dict_item; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.dict_item (
    fif_id integer NOT NULL,
    key_v character varying(100) NOT NULL,
    val character varying(100),
    ord_id integer
);


ALTER TABLE public.dict_item OWNER TO postgres;

--
-- Name: fcs; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.fcs (
    fi_id integer NOT NULL,
    ds_id integer NOT NULL,
    ct_id integer NOT NULL,
    cf_id integer NOT NULL
);


ALTER TABLE public.fcs OWNER TO postgres;

--
-- Name: ffd; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.ffd (
    fi_id integer NOT NULL,
    ds_id integer NOT NULL,
    fif_id integer NOT NULL,
    fisd_id integer
);


ALTER TABLE public.ffd OWNER TO postgres;

--
-- Name: fin_field; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.fin_field (
    fif_id integer NOT NULL,
    ident character varying(100) NOT NULL,
    title character varying(100),
    description character varying(2048)
);


ALTER TABLE public.fin_field OWNER TO postgres;

--
-- Name: fin_instrument; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.fin_instrument (
    fi_id integer NOT NULL,
    ident character varying(100) NOT NULL,
    ft_id integer NOT NULL,
    title character varying(100) DEFAULT NULL::character varying
);


ALTER TABLE public.fin_instrument OWNER TO postgres;

--
-- Name: fin_type; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.fin_type (
    ft_id integer NOT NULL,
    ident character varying(100) NOT NULL,
    title character varying(100)
);


ALTER TABLE public.fin_type OWNER TO postgres;

--
-- Name: fisd_date; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.fisd_date (
    val date,
    dat_from date NOT NULL,
    fisd_id integer NOT NULL
);


ALTER TABLE public.fisd_date OWNER TO postgres;

--
-- Name: fisd_dq; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.fisd_dq (
    val numeric,
    dat date NOT NULL,
    fisd_id integer NOT NULL
);


ALTER TABLE public.fisd_dq OWNER TO postgres;

--
-- Name: fisd_item; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.fisd_item (
    val character varying(100),
    dat_from date NOT NULL,
    fisd_id integer NOT NULL
);


ALTER TABLE public.fisd_item OWNER TO postgres;

--
-- Name: fisd_num; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.fisd_num (
    val numeric,
    dat_from date NOT NULL,
    fisd_id integer NOT NULL
);


ALTER TABLE public.fisd_num OWNER TO postgres;

--
-- Name: fisd_str; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.fisd_str (
    val character varying(100),
    dat_from date NOT NULL,
    fisd_id integer NOT NULL
);


ALTER TABLE public.fisd_str OWNER TO postgres;

--
-- Name: holidays; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.holidays (
    cal_id integer NOT NULL,
    date date NOT NULL
);


ALTER TABLE public.holidays OWNER TO postgres;

--
-- Name: mir_sequence; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.mir_sequence
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.mir_sequence OWNER TO postgres;

--
-- Name: calendar calendar_ident_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.calendar
    ADD CONSTRAINT calendar_ident_key UNIQUE (ident);


--
-- Name: calendar calendar_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.calendar
    ADD CONSTRAINT calendar_pkey PRIMARY KEY (cal_id);


--
-- Name: cashflow cashflow_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cashflow
    ADD CONSTRAINT cashflow_pkey PRIMARY KEY (cf_id, dat, valid_dat);


--
-- Name: cashflow_types cashflow_types_ident_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cashflow_types
    ADD CONSTRAINT cashflow_types_ident_key UNIQUE (ident);


--
-- Name: cashflow_types cashflow_types_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cashflow_types
    ADD CONSTRAINT cashflow_types_pkey PRIMARY KEY (ct_id);


--
-- Name: curves curves_ident_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.curves
    ADD CONSTRAINT curves_ident_key UNIQUE (ident);


--
-- Name: curves curves_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.curves
    ADD CONSTRAINT curves_pkey PRIMARY KEY (cur_id);


--
-- Name: data_source data_source_ident_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.data_source
    ADD CONSTRAINT data_source_ident_key UNIQUE (ident);


--
-- Name: data_source data_source_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.data_source
    ADD CONSTRAINT data_source_pkey PRIMARY KEY (ds_id);


--
-- Name: dict_item dict_item_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.dict_item
    ADD CONSTRAINT dict_item_pkey PRIMARY KEY (fif_id, key_v);


--
-- Name: fcs fcs_cf_id_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fcs
    ADD CONSTRAINT fcs_cf_id_key UNIQUE (cf_id);


--
-- Name: fcs fcs_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fcs
    ADD CONSTRAINT fcs_pkey PRIMARY KEY (fi_id, ds_id, ct_id);


--
-- Name: ffd fid_dis_id_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ffd
    ADD CONSTRAINT fid_dis_id_key UNIQUE (fisd_id);


--
-- Name: ffd fid_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ffd
    ADD CONSTRAINT fid_pkey PRIMARY KEY (fi_id, ds_id, fif_id);


--
-- Name: fin_field fin_field_ident_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fin_field
    ADD CONSTRAINT fin_field_ident_key UNIQUE (ident);


--
-- Name: fin_field fin_field_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fin_field
    ADD CONSTRAINT fin_field_pkey PRIMARY KEY (fif_id);


--
-- Name: fin_instrument fin_instrument_ident_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fin_instrument
    ADD CONSTRAINT fin_instrument_ident_key UNIQUE (ident);


--
-- Name: fin_instrument fin_instrument_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fin_instrument
    ADD CONSTRAINT fin_instrument_pkey PRIMARY KEY (fi_id);


--
-- Name: fin_type fin_type_ident_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fin_type
    ADD CONSTRAINT fin_type_ident_key UNIQUE (ident);


--
-- Name: fin_type fin_type_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fin_type
    ADD CONSTRAINT fin_type_pkey PRIMARY KEY (ft_id);


--
-- Name: fisd_date fisd_date_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fisd_date
    ADD CONSTRAINT fisd_date_pkey PRIMARY KEY (fisd_id, dat_from);


--
-- Name: fisd_dq fisd_dq_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fisd_dq
    ADD CONSTRAINT fisd_dq_pkey PRIMARY KEY (fisd_id, dat);


--
-- Name: fisd_item fisd_item_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fisd_item
    ADD CONSTRAINT fisd_item_pkey PRIMARY KEY (fisd_id, dat_from);


--
-- Name: fisd_num fisd_num_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fisd_num
    ADD CONSTRAINT fisd_num_pkey PRIMARY KEY (fisd_id, dat_from);


--
-- Name: fisd_str fisd_str_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fisd_str
    ADD CONSTRAINT fisd_str_pkey PRIMARY KEY (fisd_id, dat_from);


--
-- Name: holidays holidays_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.holidays
    ADD CONSTRAINT holidays_pkey PRIMARY KEY (cal_id, date);


--
-- Name: dict_item unique_key_val; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.dict_item
    ADD CONSTRAINT unique_key_val UNIQUE (key_v);


--
-- Name: calendar upper_foo_on_insert_trigger; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER upper_foo_on_insert_trigger BEFORE INSERT OR UPDATE ON public.calendar FOR EACH ROW EXECUTE PROCEDURE public.upper_foo_on_insert();


--
-- Name: curves upper_foo_on_insert_trigger; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER upper_foo_on_insert_trigger BEFORE INSERT OR UPDATE ON public.curves FOR EACH ROW EXECUTE PROCEDURE public.upper_foo_on_insert();


--
-- Name: data_source upper_foo_on_insert_trigger; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER upper_foo_on_insert_trigger BEFORE INSERT OR UPDATE ON public.data_source FOR EACH ROW EXECUTE PROCEDURE public.upper_foo_on_insert();


--
-- Name: fin_field upper_foo_on_insert_trigger; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER upper_foo_on_insert_trigger BEFORE INSERT OR UPDATE ON public.fin_field FOR EACH ROW EXECUTE PROCEDURE public.upper_foo_on_insert();


--
-- Name: fin_instrument upper_foo_on_insert_trigger; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER upper_foo_on_insert_trigger BEFORE INSERT OR UPDATE ON public.fin_instrument FOR EACH ROW EXECUTE PROCEDURE public.upper_foo_on_insert();


--
-- Name: fin_type upper_foo_on_insert_trigger; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER upper_foo_on_insert_trigger BEFORE INSERT OR UPDATE ON public.fin_type FOR EACH ROW EXECUTE PROCEDURE public.upper_foo_on_insert();


--
-- Name: cashflow_types upper_foo_on_insert_trigger; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER upper_foo_on_insert_trigger BEFORE INSERT OR UPDATE ON public.cashflow_types FOR EACH ROW EXECUTE PROCEDURE public.upper_foo_on_insert();


--
-- Name: cashflow cashflow_cf_id_foreign_key; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cashflow
    ADD CONSTRAINT cashflow_cf_id_foreign_key FOREIGN KEY (cf_id) REFERENCES public.fcs(cf_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: curve_list curve_list_cur_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.curve_list
    ADD CONSTRAINT curve_list_cur_id FOREIGN KEY (cur_id) REFERENCES public.curves(cur_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: curve_list curve_list_fi_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.curve_list
    ADD CONSTRAINT curve_list_fi_id FOREIGN KEY (fi_id) REFERENCES public.fin_instrument(fi_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: dict_item dict_item_fif_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.dict_item
    ADD CONSTRAINT dict_item_fif_id FOREIGN KEY (fif_id) REFERENCES public.fin_field(fif_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: ffd ds_id_fid; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ffd
    ADD CONSTRAINT ds_id_fid FOREIGN KEY (ds_id) REFERENCES public.data_source(ds_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: fcs fcs_ct_id_fid; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fcs
    ADD CONSTRAINT fcs_ct_id_fid FOREIGN KEY (ct_id) REFERENCES public.cashflow_types(ct_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: fcs fcs_ds_id_fid; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fcs
    ADD CONSTRAINT fcs_ds_id_fid FOREIGN KEY (ds_id) REFERENCES public.data_source(ds_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: fcs fcs_fi_id_fid; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fcs
    ADD CONSTRAINT fcs_fi_id_fid FOREIGN KEY (fi_id) REFERENCES public.fin_instrument(fi_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: ffd fi_id_fid; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ffd
    ADD CONSTRAINT fi_id_fid FOREIGN KEY (fi_id) REFERENCES public.fin_instrument(fi_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: ffd fif_id_fid; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ffd
    ADD CONSTRAINT fif_id_fid FOREIGN KEY (fif_id) REFERENCES public.fin_field(fif_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: fin_instrument fin_instrument_ft_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fin_instrument
    ADD CONSTRAINT fin_instrument_ft_id FOREIGN KEY (ft_id) REFERENCES public.fin_type(ft_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: fisd_item fisd_item_val; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.fisd_item
    ADD CONSTRAINT fisd_item_val FOREIGN KEY (val) REFERENCES public.dict_item(key_v) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: holidays holidays_cal_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.holidays
    ADD CONSTRAINT holidays_cal_id FOREIGN KEY (cal_id) REFERENCES public.calendar(cal_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

