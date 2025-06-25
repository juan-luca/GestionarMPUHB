//using Anses.Director.Session;
using Anses.Director.Session;
using Lupa.Contrato;
using Lupa.Contrato.Lupa;
using Lupa.DB2;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Xml;

namespace WS.MPU.Negocio
{
    public class Utils
    {

        private static string Padre = ConfigurationManager.AppSettings["PADRE"].ToString();
        #region Declaracion de Servicios

        public static TurnosWSV2.BasicHttpBinding_IServiceTurnos ObtenerServicioTurnos()
        {
            try
            {
                TurnosWSV2.BasicHttpBinding_IServiceTurnos srv = new TurnosWSV2.BasicHttpBinding_IServiceTurnos();
                srv.Url = ConfigurationManager.AppSettings["TurnosWSV2"];
                srv.Credentials = System.Net.CredentialCache.DefaultCredentials;
                return srv;
            }
            catch (Exception ex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message, Environment.NewLine, ex.StackTrace), System.Diagnostics.TraceEventType.Error);
                throw new Exception("Error en la consulta al servicio de relaciones");
            }
        }

        public static Wspw04.WS_PW04 ObtenerServicioRelaciones()
        {
            try
            {
                Wspw04.WS_PW04 srv = new Wspw04.WS_PW04();
                srv.Url = ConfigurationManager.AppSettings[(srv.GetType().ToString().Substring(13))];
                srv.Credentials = System.Net.CredentialCache.DefaultCredentials;
                return srv;
            }
            catch (Exception ex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message, Environment.NewLine, ex.StackTrace), System.Diagnostics.TraceEventType.Error);
                throw new Exception("Error en la consulta al servicio de relaciones");
            }
        }
        public static EnviarMail.EnviarMail ObtenerEnvioDeMail()
        {
            try
            {
                EnviarMail.EnviarMail srv = new EnviarMail.EnviarMail();
                srv.Url = ConfigurationManager.AppSettings[(srv.GetType().ToString().Substring(13))];
                srv.Credentials = System.Net.CredentialCache.DefaultCredentials;
                return srv;
            }
            catch (Exception ex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message, Environment.NewLine, ex.StackTrace), System.Diagnostics.TraceEventType.Error);
                throw new Exception("Error en el envio de mails");
            }
        }
        public static ValidarCBU.ValidarCBU ObtenerValidacionCBU()
        {
            try
            {
                ValidarCBU.ValidarCBU srv = new ValidarCBU.ValidarCBU();
                srv.Url = ConfigurationManager.AppSettings[(srv.GetType().ToString().Substring(13))];
                srv.Credentials = System.Net.CredentialCache.DefaultCredentials;
                return srv;
            }
            catch (Exception ex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message, Environment.NewLine, ex.StackTrace), System.Diagnostics.TraceEventType.Error);
                throw new Exception("Error en el servicio de verificacion de CBU");
            }
        }
        public static adpWS.DatosdePersonaporCuip ObtenerServicioDatosPersonaxCuip()
        {
            try
            {
                adpWS.DatosdePersonaporCuip srv = new adpWS.DatosdePersonaporCuip();
                srv.Url = ConfigurationManager.AppSettings[(srv.GetType().ToString().Substring(13))];
                srv.Credentials = System.Net.CredentialCache.DefaultCredentials;
                return srv;
            }
            catch (Exception ex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message, Environment.NewLine, ex.StackTrace), System.Diagnostics.TraceEventType.Error);
                throw new Exception("Error en la consulta al servicio de persona");
            }
        }
        public static ADPDescripciones.ADPDescripciones ObtenerServicioDescripciones()
        {
            try
            {
                ADPDescripciones.ADPDescripciones srv = new ADPDescripciones.ADPDescripciones();
                srv.Url = ConfigurationManager.AppSettings[(srv.GetType().ToString().Substring(13))];
                srv.Credentials = System.Net.CredentialCache.DefaultCredentials;
                return srv;
            }
            catch (Exception ex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message, Environment.NewLine, ex.StackTrace), System.Diagnostics.TraceEventType.Error);
                throw new Exception("Error en la consulta al servicio de persona");
            }
        }
        public static ServicioAEZD.ServicioAEZD ObtenerServicioEmpresas()
        {
            try
            {
                ServicioAEZD.ServicioAEZD srv = new ServicioAEZD.ServicioAEZD();
                srv.Url = ConfigurationManager.AppSettings[(srv.GetType().ToString().Substring(13))];
                srv.Credentials = System.Net.CredentialCache.DefaultCredentials;
                return srv;
            }
            catch (Exception ex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message, Environment.NewLine, ex.StackTrace), System.Diagnostics.TraceEventType.Error);
                throw new Exception("Error en la consulta al servicio de persona");
            }
        }
        #endregion

        #region TraerRelaciones
        public static List<RelacionesDepJud> TraerRelaciones(string cuil)
        {
            Wspw04.WS_PW04 servicioRelaciones = ObtenerServicioRelaciones();
            Wspw04.ListaPw04 relaciones = new Wspw04.ListaPw04();
            adpWS.RetornoDatosPersonaCuip datos = new adpWS.RetornoDatosPersonaCuip();
            adpWS.RetornoDatosPersonaCuip Titular = new adpWS.RetornoDatosPersonaCuip();

            short pagina = 1;
            int RegistrosLeidos = 0;

            List<RelacionesDepJud> relacionesTitular = new List<RelacionesDepJud>();

            relaciones = servicioRelaciones.ObtenerRelacionesxCuil(cuil, pagina);
            Titular = PersonaNegocio.TraerDatosPersonaxCuil(cuil);

            //armo un listado de relacion si son validas
            while (RegistrosLeidos < relaciones.tot_ocur)
            {
                //valido que el listado sea valido
                if (relaciones.cod_retorno == 0)
                {
                    relaciones.Lista.ToList().ForEach(relacion =>
                    {
                        datos = PersonaNegocio.TraerDatosPersonaxCuil(relacion.cuil_rela);
                        if (datos.PersonaCuip != null)
                        {
                            RelacionesDepJud Relacion = new RelacionesDepJud()
                            {
                                Cuil_Relacionado = decimal.Parse(relacion.cuil_rela),
                                ApellidoYNombre = relacion.ape_nom,
                                EstadoCivil = datos.PersonaCuip.cod_estcivil.ToString() == "0" ? "Estado 0 en ADP" : LUPADB2.CargarEstCivil().Single(x => x.Key == short.Parse(datos.PersonaCuip.cod_estcivil.ToString())).Value,
                                FNacimiento = relacion.f_naci,
                                FFallecimiento = datos.PersonaCuip.f_falle == new DateTime() ? (DateTime?)null : datos.PersonaCuip.f_falle,
                                Relacion = relacion.da_relacion,
                                codigoRelacion = relacion.c_relacion,
                                DeclaracionJurada = null,
                                Incapacidad = (datos.PersonaCuip.cod_incap == 9000 || datos.PersonaCuip.cod_incap == 9001) ? 'S' : 'N',
                                Residente = null,
                                Cuil_Embargado = null,
                                Fallecido = datos.PersonaCuip.f_falle == new DateTime() ? false : true,
                                EsMenor18 = PersonaNegocio.EsMenor18(relacion.f_naci),
                                EsMenor16 = PersonaNegocio.EsMenor16(relacion.f_naci),
                                FechaInicioRelacion = relacion.f_desde == "" ? "" : relacion.f_desde.Substring(0, 2) + "/" + relacion.f_desde.Substring(2, 2) + "/" + relacion.f_desde.Substring(4),
                                FechaHastaVigenciaRelacion = relacion.f_vig_hasta == "" ? "" : relacion.f_vig_hasta.Substring(0, 2) + "/" + relacion.f_vig_hasta.Substring(2, 2) + "/" + relacion.f_vig_hasta.Substring(4)
                            };
                            relacionesTitular.Add(Relacion);
                        }
                    });
                }
                else
                {
                    throw new Exception("Error al consultar el listado de relaciones");
                }
                //actualizo condicion del ciclo
                RegistrosLeidos = RegistrosLeidos + relaciones.Lista.Count();
                pagina++;
                relaciones = servicioRelaciones.ObtenerRelacionesxCuil(cuil, pagina);
            }
            //devuelvo relaciones
            return relacionesTitular;
        }
        public static List<RelacionesDepJud> TraerRelacionesEnComun(string cuil, string cuil_conyuge, decimal? PeriodoDesde, decimal? PeriodoHasta)
        {
            List<RelacionesDepJud> Relaciones = new List<RelacionesDepJud>();
            List<RelacionesDepJud> RelacionesConyuge = new List<RelacionesDepJud>();
            List<RelacionesDepJud> RelacionesEnComun = new List<RelacionesDepJud>();

            Relaciones = TraerListaRelaciones(cuil, PeriodoDesde);
            //si el cuil conyuge es null trae solo las relaciones del cuil, sino trae la des conyuge y devuelve las que el cuil rel sea igual
            if (cuil_conyuge != null)
            {
                RelacionesConyuge = TraerListaRelaciones(cuil_conyuge, PeriodoDesde);

                Relaciones.ForEach(Rel => RelacionesConyuge.ForEach(RelConyuge =>
                {
                    if (Rel.Cuil_Relacionado == RelConyuge.Cuil_Relacionado)
                    {
                        Rel.RelacionConEmbargado = RelConyuge.Relacion;
                        RelacionesEnComun.Add(Rel);
                    }
                })
                );
                RelacionesEnComun = LUPANegocio.DepositoRenunciaVigenteXCuilito(RelacionesEnComun, PeriodoDesde, PeriodoHasta);
                RelacionesEnComun = LUPADB2.NovedadRenunciaCuilTCuilR(RelacionesEnComun, long.Parse(cuil), PeriodoDesde, PeriodoHasta);
                return RelacionesEnComun;
            }
            else
            {
                Relaciones = LUPANegocio.DepositoRenunciaVigenteXCuilito(Relaciones, PeriodoDesde, PeriodoHasta);
                Relaciones = LUPADB2.NovedadRenunciaCuilTCuilR(Relaciones, long.Parse(cuil), PeriodoDesde, PeriodoHasta);
                return Relaciones;
            }
        }
        public static List<RelacionesDepJud> TraerListaRelaciones(string cuil, decimal? PeriodoDesde)
        {
            Wspw04.WS_PW04 servicioRelaciones = ObtenerServicioRelaciones();
            Wspw04.ListaPw04 relaciones = new Wspw04.ListaPw04();
            adpWS.RetornoDatosPersonaCuip datos = new adpWS.RetornoDatosPersonaCuip();
            adpWS.RetornoDatosPersonaCuip Titular = new adpWS.RetornoDatosPersonaCuip();

            short pagina = 1;
            int RegistrosLeidos = 0;

            List<RelacionesDepJud> relacionesTitular = new List<RelacionesDepJud>();

            relaciones = servicioRelaciones.ObtenerRelacionesxCuil(cuil, pagina);
            Titular = PersonaNegocio.TraerDatosPersonaxCuil(cuil);

            //valida que el titular no este fallecido
            if (FallecidoPeriodoDesde(PeriodoDesde, Titular.PersonaCuip.f_falle, Titular.PersonaCuip.cod_falleci))
            {
                return relacionesTitular;
            }

            //armo un listado de relacion si son validas
            while (RegistrosLeidos < relaciones.tot_ocur)
            {
                //valido que el listado sea valido
                if (relaciones.cod_retorno == 0)
                {
                    relaciones.Lista.ToList().ForEach(relacion =>
                    {
                        datos = PersonaNegocio.TraerDatosPersonaxCuil(relacion.cuil_rela);
                        if (datos.PersonaCuip != null)
                        {
                            //valida que el relacionado no este fallecido
                            if (!FallecidoPeriodoDesde(PeriodoDesde, datos.PersonaCuip.f_falle, datos.PersonaCuip.cod_falleci))    //((PeriodoDesde == (Decimal?)null || PeriodoDesde == 0) ? true : datos.PersonaCuip.cod_falleci == 0 && datos.PersonaCuip.f_falle == new DateTime() ? true : datos.PersonaCuip.f_falle > PeriodoAFecha(PeriodoDesde.Value))
                            {
                                if (RelacionVigentePeDesde(relacion, PeriodoDesde))  //(PeriodoDesde == (Decimal?)null || PeriodoDesde == 0 ? true : long.Parse(relacion.f_desde.Substring(4) + relacion.f_desde.Substring(2, 2)) < PeriodoDesde && (relacion.f_vig_hasta.Length < 8 ? true : long.Parse(relacion.f_vig_hasta.Substring(4) + relacion.f_vig_hasta.Substring(2, 2)) > PeriodoDesde))
                                {
                                    if (EsRelacionValidaRenunciaDeposito(relacion.c_relacion, relacion.da_relacion))
                                    {
                                        RelacionesDepJud Relacion = new RelacionesDepJud()
                                        {
                                            Cuil_Relacionado = decimal.Parse(relacion.cuil_rela),
                                            ApellidoYNombre = relacion.ape_nom,
                                            EstadoCivil = datos.PersonaCuip.cod_estcivil.ToString() == "0" ? "Estado 0 en ADP" : LUPADB2.CargarEstCivil().Single(x => x.Key == short.Parse(datos.PersonaCuip.cod_estcivil.ToString())).Value,
                                            FNacimiento = relacion.f_naci,
                                            FFallecimiento = datos.PersonaCuip.f_falle == new DateTime() ? (DateTime?)null : datos.PersonaCuip.f_falle,
                                            Relacion = relacion.da_relacion,
                                            codigoRelacion = relacion.c_relacion,
                                            DeclaracionJurada = null,
                                            Incapacidad = (datos.PersonaCuip.cod_incap == 9000 || datos.PersonaCuip.cod_incap == 9001) ? 'S' : 'N',
                                            Residente = null,
                                            Cuil_Embargado = null,
                                            Fallecido = datos.PersonaCuip.f_falle == new DateTime() ? false : true,
                                            EsMenor18 = PersonaNegocio.EsMenorEdadPeriodoDedse(relacion.f_naci, PeriodoDesde.Value),
                                            EsMenor16 = PersonaNegocio.EsMenor16(relacion.f_naci),
                                            FechaInicioRelacion = relacion.f_desde == "" ? "" : relacion.f_desde.Substring(0, 2) + "/" + relacion.f_desde.Substring(2, 2) + "/" + relacion.f_desde.Substring(4),
                                            FechaHastaVigenciaRelacion = relacion.f_vig_hasta == "" ? "" : relacion.f_vig_hasta.Substring(0, 2) + "/" + relacion.f_vig_hasta.Substring(2, 2) + "/" + relacion.f_vig_hasta.Substring(4)
                                        };
                                        //si la relacione no existe la agrega, sino se queda solo con la mas relevante
                                        EliminarRelacionesDuplicadas(Relacion, relacionesTitular);
                                        //relacionesTitular.Add(Relacion);
                                    }
                                }
                            }
                        }
                    });
                }
                else
                {
                    throw new Exception("Error al consultar el listado de relaciones");
                }
                //actualizo condicion del ciclo
                RegistrosLeidos = RegistrosLeidos + relaciones.Lista.Count();
                pagina++;
                relaciones = servicioRelaciones.ObtenerRelacionesxCuil(cuil, pagina);
            }
            //devuelvo relaciones
            return relacionesTitular;
        }
        public static List<RelacionesDepJud> TraerRelacionesDHPorCuil(string cuil, DateTime? FechaFallecimiento)
        {
            Wspw04.WS_PW04 servicioRelaciones = ObtenerServicioRelaciones();
            Wspw04.ListaPw04 relaciones = new Wspw04.ListaPw04();
            adpWS.RetornoDatosPersonaCuip datos = new adpWS.RetornoDatosPersonaCuip();

            short pagina = 1;
            int RegistrosLeidos = 0;

            List<RelacionesDepJud> relacionesTitular = new List<RelacionesDepJud>();

            relaciones = servicioRelaciones.ObtenerRelacionesxCuil(cuil, pagina);

            //armo un listado de relacion si son validas
            while (RegistrosLeidos < relaciones.tot_ocur)
            {
                //valido el retorno del servicio de relaciones
                if (relaciones.cod_retorno == 0)
                {
                    relaciones.Lista.ToList().ForEach(relacion =>
                    {
                        datos = PersonaNegocio.TraerDatosPersonaxCuil(relacion.cuil_rela);
                        if (datos.PersonaCuip != null)
                        {
                            //si el codigo de relacion es valido y la fecha desde relacion es anterior a la fecha de fallecimiento
                            if (EsRelacionValidaDerechoHabiente(relacion.c_relacion, relacion.da_relacion) && (FechaFallecimiento.HasValue ? Utils.FormatDateSinBarras(relacion.f_desde) <= FechaFallecimiento : true))
                            {
                                RelacionesDepJud Relacion = new RelacionesDepJud()
                                {
                                    Cuil_Relacionado = decimal.Parse(relacion.cuil_rela),
                                    ApellidoYNombre = relacion.ape_nom,
                                    EstadoCivil = datos.PersonaCuip.cod_estcivil.ToString() == "0" ? "Estado 0 en ADP" : LUPADB2.CargarEstCivil().Single(x => x.Key == short.Parse(datos.PersonaCuip.cod_estcivil.ToString())).Value,
                                    FNacimiento = relacion.f_naci,
                                    FFallecimiento = datos.PersonaCuip.f_falle == new DateTime() ? (DateTime?)null : datos.PersonaCuip.f_falle,
                                    Relacion = relacion.da_relacion,
                                    codigoRelacion = relacion.c_relacion,
                                    DeclaracionJurada = null,
                                    Incapacidad = (datos.PersonaCuip.cod_incap == 9000 || datos.PersonaCuip.cod_incap == 9001) ? 'S' : 'N',
                                    Residente = null,
                                    Cuil_Embargado = null,
                                    Fallecido = datos.PersonaCuip.f_falle == new DateTime() ? false : true,
                                    EsMenor18 = PersonaNegocio.EsMenor18(relacion.f_naci),
                                    EsMenor16 = PersonaNegocio.EsMenor16(relacion.f_naci),
                                    FechaInicioRelacion = relacion.f_desde == "" ? "" : relacion.f_desde.Substring(0, 2) + "/" + relacion.f_desde.Substring(2, 2) + "/" + relacion.f_desde.Substring(4),
                                    FechaHastaVigenciaRelacion = relacion.f_vig_hasta == "" ? "" : relacion.f_vig_hasta.Substring(0, 2) + "/" + relacion.f_vig_hasta.Substring(2, 2) + "/" + relacion.f_vig_hasta.Substring(4)
                                };
                                relacionesTitular.Add(Relacion);
                            }
                        }
                    });
                }
                else
                {
                    throw new Exception("Error al consultar las relaciones");
                }
                //actualizo condicion del ciclo
                RegistrosLeidos = relaciones.Lista.Count();
                pagina++;
                relaciones = servicioRelaciones.ObtenerRelacionesxCuil(cuil, pagina);
            }
            //devuelvo relaciones
            return relacionesTitular;
        }
        public static RelacionesDH RelacionesDh(string cuilFallecido, string cuilSolicitante, DateTime FechaPresentacion)
        {
            try
            {
                List<Hijo> relacionesEnComun = new List<Hijo>();
                RelacionesDH RDH = new RelacionesDH();
                List<DerechoHabiente> derechos = new List<DerechoHabiente>();
                RDH.mensajes = new List<string>();

                List<RelacionesDepJud> relacionesSolicitante = new List<RelacionesDepJud>();
                List<RelacionesDepJud> relacionesFallecido = new List<RelacionesDepJud>();
                List<RelacionesDepJud> relacionesOtroProg = new List<RelacionesDepJud>();

                #region datos del Fallecido
                adpWS.RetornoDatosPersonaCuip fallecido = new adpWS.RetornoDatosPersonaCuip();

                if (Validaciones.ValidarNumero(cuilFallecido.ToString()))
                {
                    fallecido = PersonaNegocio.TraerDatosPersonaxCuil(cuilFallecido);
                    RDH.DocumentoFallecido = fallecido.PersonaCuip.doc_nro;
                    RDH.TipoDocumento = fallecido.PersonaCuip.doc_c_tipo.ToString();
                    RDH.NombreYApeF = fallecido.PersonaCuip.ape_nom;
                }
                else
                {
                    RDH.Error = true;
                    RDH.mensajes.Add("El cuil " + cuilFallecido + " no tiene formato valido");
                    RDH.ConfirmaRechazaTramite = false;
                    return RDH;
                }
                #endregion

                //si la fecha de fallecimiento es distinta de new date valido que sea menor a la fecha de presentacion. Caso contrario el tramite no es valido
                if (fallecido.PersonaCuip.f_falle != new DateTime() && fallecido.PersonaCuip.f_falle < FechaPresentacion)
                {
                    //traigo las relaciones del fallecido
                    relacionesFallecido = TraerRelacionesDHPorCuil(cuilFallecido, fallecido.PersonaCuip.f_falle);
                    //traigo el listado de tramites para esa persona para validar que no tenga otro con ese cuil fallecido
                    derechos = DerechoHabienteNegocio.DerechoLista(long.Parse(cuilSolicitante), null);

                    //si se encuentra un tramite ya cargado de derecho habiente vigente para esos 2 cuiles
                    if (derechos.Find(derecho => derecho.cuilTitular == long.Parse(cuilFallecido) && derecho.AptoBaja == true) == null)
                    {
                        if (relacionesFallecido != null)
                        {
                            //Se fija si la relacion entre el fallecido y el solicitante es conyuge
                            try
                            {
                                //traigo el conyuge valido a la fecha de fallecimiento, la fecha hasta vigencia de la relacion tiene que ser vacia o menor igual a la de fallecimiento no puede ser anterior
                                List<RelacionesDepJud> conyuge = TraerConyugeConviviente(cuilFallecido, fallecido.PersonaCuip.f_falle);
                                if (conyuge.Count() > 0)
                                {
                                    RDH.TipoRelacion = conyuge.Single(e => e.Cuil_Relacionado == decimal.Parse(cuilSolicitante)).Relacion;
                                    if (conyuge.Any(e => e.Cuil_Relacionado == decimal.Parse(cuilSolicitante)))
                                    {
                                        RDH.EsConyuge = true;
                                    }
                                }
                            }
                            catch
                            {
                                RDH.EsConyuge = false;
                            }

                            //genera una lista de todas las relaciones en comun entre el solicitante y el fallecido
                            List<RelacionesDepJud> listasRelaciones = new List<RelacionesDepJud>();



                            relacionesSolicitante = TraerRelacionesDHPorCuil(cuilSolicitante, fallecido.PersonaCuip.f_falle);
                            if (relacionesSolicitante.Count > 0)
                            {
                                listasRelaciones = relacionesSolicitante.Where(e => relacionesFallecido.Any(i => i.Cuil_Relacionado == e.Cuil_Relacionado)).ToList();

                                //cargo los prenatales o materninades que podria tener
                                //RelacionesDH PrenaMat = TienePrenaMat(fallecido, cuilFallecido);
                                //RDH.TienePrenatal = PrenaMat.TienePrenatal;
                                //RDH.Prenatal = PrenaMat.Prenatal;
                                //RDH.TieneMaternidad = PrenaMat.TieneMaternidad;
                                //RDH.Maternidad = PrenaMat.Maternidad;
                            }

                            if (!RDH.EsConyuge)
                            {
                                //Si no es Conyuge busca si tiene una relacion de hijo con el cuil solicitante

                                /*
                                try
                                {
                                    relacionesSolicitante.Add(relacionesFallecido.Where(RelFalle => 
                                        RelFalle.Cuil_Relacionado.ToString() == cuilSolicitante && 
                                        RelFalle.codigoRelacion == 3 && 
                                        RelFalle.Relacion == "HIJO/A").First());
                                }
                                catch (Exception e)
                                {
                                    //relacionesSolicitante = new List<RelacionesDepJud>();
                                }
                                */
                                relacionesSolicitante = relacionesSolicitante.Where(d => d.Cuil_Relacionado.ToString() == cuilFallecido
                                        && d.codigoRelacion == 3
                                ).ToList();


                                //si tiene un relacion de hijo
                                if (relacionesSolicitante.Count > 0)
                                {
                                    //trae la lista de hijos en comun entre el DH y el otro progenitor (codigo 3 padre) del relacionado
                                    listasRelaciones.AddRange(traerGFDH(relacionesFallecido, TraerCuilOtroProgenitor(cuilSolicitante, cuilFallecido), fallecido.PersonaCuip.f_falle));

                                    if (listasRelaciones.Count == 0)
                                    {
                                        listasRelaciones.Add(relacionesSolicitante.First());
                                    }
                                }
                            }

                            //si no hay ninguna relacion no es valido el tramite
                            if (listasRelaciones.Count == 0)
                            {
                                RDH.Error = true;
                                RDH.mensajes.Add("No posee relación válida con el cuil fallecido");
                                RDH.ConfirmaRechazaTramite = false;
                                return RDH;
                            }

                            foreach (var relacion in listasRelaciones)
                            {
                                //valido que el relacionado sea valido
                                adpWS.RetornoDatosPersonaCuip datos = PersonaNegocio.TraerDatosPersonaxCuil(relacion.Cuil_Relacionado.ToString());

                                if (datos.PersonaCuip != null)
                                {
                                    Hijo relacionado = new Hijo();
                                    relacionado.Cuil = relacion.Cuil_Relacionado.ToString();
                                    relacionado.Nombre = datos.PersonaCuip.ape_nom;
                                    relacionado.Fhasta = null;
                                    relacionesEnComun.Add(relacionado);
                                }
                            }
                        }
                        else
                        {
                            RDH.Error = true;
                            RDH.mensajes.Add("El cuil " + cuilFallecido + " no posee grupo familiar");
                            RDH.ConfirmaRechazaTramite = false;
                        }
                    }
                    else
                    {
                        RDH.Error = true;
                        RDH.mensajes.Add("Ya posee un trámite de derecho habiente para el cuil" + cuilFallecido);
                        RDH.ConfirmaRechazaTramite = false;
                    }
                    RDH.CuilFallecido = cuilFallecido;
                    RDH.CuilRelacionado = relacionesEnComun;
                }
                else
                {
                    RDH.Error = true;
                    RDH.mensajes.Add("El cuil solicitado no se encuentra fallecido a la fecha de presentación");
                    RDH.ConfirmaRechazaTramite = false;
                }
                return RDH;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error en los servicios de consulta de Relaciones - {0} - Error:{1}->{2}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message));
            }
        }
        public static List<RelacionesDepJud> TraerConyugeConviviente(string cuil, DateTime Fecha)
        {
            List<RelacionesDepJud> conyuges = new List<RelacionesDepJud>();
            short pagina = 1;
            int cantidadRegistros = 0;
            int RegistrosLeidos = 0;
            try
            {
                Wspw04.WS_PW04 ServicioRelaciones = ObtenerServicioRelaciones();
                Wspw04.ListaPw04 Relaciones = ServicioRelaciones.ObtenerRelacionesxCuil(cuil, pagina);

                if (Relaciones.cod_retorno != 0)
                {
                    return new List<RelacionesDepJud>();
                }

                cantidadRegistros = Relaciones.Lista.Length;

                while (RegistrosLeidos < cantidadRegistros)
                {
                    //valida la relacion a la fecha de fallecimiento, la fecha hasta vigencia tiene que ser nula o mayor y igual a la fecha de fallecimiento, no puede ser anterior
                    Relaciones.Lista.ToList().Where(X => X.c_relacion == 1 || X.c_relacion == 2 || X.c_relacion == 25).ToList().ForEach(relacion =>
                    {
                        adpWS.RetornoDatosPersonaCuip datos = PersonaNegocio.TraerDatosPersonaxCuil(relacion.cuil_rela);

                        RelacionesDepJud rel = new RelacionesDepJud()
                        {
                            Cuil_Relacionado = decimal.Parse(relacion.cuil_rela),
                            ApellidoYNombre = relacion.ape_nom,
                            EstadoCivil = datos.PersonaCuip.cod_estcivil.ToString() == "0" ? "Estado 0 en ADP" : LUPADB2.CargarEstCivil().Single(x => x.Key == short.Parse(datos.PersonaCuip.cod_estcivil.ToString())).Value,
                            FNacimiento = relacion.f_naci,
                            FFallecimiento = datos.PersonaCuip.f_falle == new DateTime() ? (DateTime?)null : datos.PersonaCuip.f_falle,
                            Relacion = relacion.da_relacion,
                            codigoRelacion = relacion.c_relacion,
                            DeclaracionJurada = null,
                            Incapacidad = (datos.PersonaCuip.cod_incap == 9000 || datos.PersonaCuip.cod_incap == 9001) ? 'S' : 'N',
                            Residente = null,
                            Cuil_Embargado = null,
                            Fallecido = datos.PersonaCuip.cod_falleci == 0 ? false : true,
                            EsMenor18 = PersonaNegocio.EsMenorEdadFechaPresentacion(relacion.f_naci, Fecha),
                            EsMenor16 = PersonaNegocio.EsMenor16(relacion.f_naci),
                            FechaInicioRelacion = relacion.f_desde == "" ? "" : relacion.f_desde.Substring(0, 2) + "/" + relacion.f_desde.Substring(2, 2) + "/" + relacion.f_desde.Substring(4),
                            FechaHastaVigenciaRelacion = relacion.f_vig_hasta == "" ? "" : relacion.f_vig_hasta.Substring(0, 2) + "/" + relacion.f_vig_hasta.Substring(2, 2) + "/" + relacion.f_vig_hasta.Substring(4)
                        };

                        rel.ErrorADP = ErroresAdpRelacion(datos, relacion.leg_unico, relacion.f_desde, relacion.f_vig_hasta, Fecha);

                        conyuges.Add(rel);
                    });
                    RegistrosLeidos = RegistrosLeidos + Relaciones.Lista.Length;
                    pagina++;
                    ServicioRelaciones.ObtenerRelacionesxCuil(cuil, pagina);
                }
            }
            catch (Exception ex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message), System.Diagnostics.TraceEventType.Error);
            }
            return conyuges;//LUPANegocio.DepositoRenunciaVigenteXCuilito(conyuges, PeriodoDesde, PeriodoHasta);
        }
        public static string TraerCuilOtroProgenitor(string cuilSolicitante, string cuilProgenitor)
        {
            Wspw04.WS_PW04 ws = ObtenerServicioRelaciones();
            List<RelacionesDepJud> relacionesSolicitante = TraerRelaciones(cuilSolicitante);
            try
            {
                return relacionesSolicitante.Where(e => e.Cuil_Relacionado.ToString() != cuilProgenitor && e.codigoRelacion == 3 && e.Relacion == Padre).First().Cuil_Relacionado.ToString();
            }
            catch
            {
                return "";
            }
        }
        public static List<RelacionesDepJud> traerGFDH(List<RelacionesDepJud> relacionesFallecido, string cuilProg2, DateTime FechaFallecimiento)
        {
            List<RelacionesDepJud> GF = new List<RelacionesDepJud>();
            if (cuilProg2 != "")
            {
                List<RelacionesDepJud> RelacionesOtroProg = TraerRelacionesDHPorCuil(cuilProg2, FechaFallecimiento);

                relacionesFallecido.ForEach(RF => RelacionesOtroProg.ForEach(RO =>
                {
                    if (RO.Cuil_Relacionado == RF.Cuil_Relacionado && RO.codigoRelacion == RF.codigoRelacion)
                    {
                        if (DateTime.Parse(RO.FechaInicioRelacion) <= FechaFallecimiento && (RO.FechaHastaVigenciaRelacion == "" || RO.FechaHastaVigenciaRelacion == null ? true : DateTime.Parse(RO.FechaHastaVigenciaRelacion) >= FechaFallecimiento))
                        {
                            GF.Add(RF);
                        }
                    }
                }));
            }
            return GF;
        }
        public static List<RelacionesDepJud> EliminarRelacionesDuplicadas(RelacionesDepJud Relacion, List<RelacionesDepJud> Relaciones)
        {
            try
            {
                RelacionesDepJud RelTemp = new RelacionesDepJud();
                try
                {
                    //busca entre la relaciones si ya existe una relacion para ese par cuil relacionado cuil embargado
                    RelTemp = Relaciones.Find(rel => rel.Cuil_Relacionado == Relacion.Cuil_Relacionado && rel.Cuil_Embargado == Relacion.Cuil_Embargado);
                }
                catch
                {
                    Relaciones.Add(Relacion);
                }

                if (RelTemp == null)
                {
                    //si no existe la agrega
                    Relaciones.Add(Relacion);
                }
                else
                {
                    //si la fecha hasta es null la agregar y saca la anterior
                    if (string.IsNullOrEmpty(Relacion.FechaHastaVigenciaRelacion))
                    {
                        Relaciones.Remove(RelTemp);
                        Relaciones.Add(Relacion);
                    }
                    //si la fecha anterior no es nulla
                    else if (!string.IsNullOrEmpty(RelTemp.FechaHastaVigenciaRelacion))
                    {
                        //si la fecha hasta es mayor a la anterior lo mismo 
                        if (DateTime.Parse(RelTemp.FechaHastaVigenciaRelacion) < DateTime.Parse(Relacion.FechaHastaVigenciaRelacion))
                        {
                            Relaciones.Remove(RelTemp);
                            Relaciones.Add(Relacion);
                        }
                    }
                }
            }
            catch
            {
                Relaciones.Add(Relacion);
            }
            return Relaciones;
        }
        #endregion

        #region ConsultaDatosExternos
        public static string TraerRazonSocial(long Cuit)
        {
            try
            {
                //cargo los datos del empleador o empresa
                var ServicioEmpresas = Utils.ObtenerServicioEmpresas();
                ServicioAEZD.ResultadoOfDatoBasicoEmpresa RespuestaDatosEmpresa = new ServicioAEZD.ResultadoOfDatoBasicoEmpresa();
                RespuestaDatosEmpresa = ServicioEmpresas.TraerDatosBasicosDeEmpresa(Cuit.ToString(), "C");
                ServicioAEZD.DatoBasicoEmpresa[] datosEmpresa = RespuestaDatosEmpresa.Datos;
                return datosEmpresa[0].RazonSocial;
            }
            catch
            {
                throw new Exception("ocurrio un error durante la consulta al servicio de ADE");
            }
        }
        public static string TraerDatosSIPAxCuil(long cuil)
        {
            return "datos";
        }
        public static bool ErroresAdpRelacion(adpWS.RetornoDatosPersonaCuip datos, string legajo, string f_desde, string f_vig_hasta, DateTime Fecha)
        {
            try
            {
                return !(datos.error.cod_retorno == 0 && (legajo == "N" || legajo == "X") && FormatDateSinBarras(f_desde) <= Fecha && (f_vig_hasta == "" || DateTime.Parse(f_vig_hasta) >= Fecha));
            }
            catch
            {
                return true;
            }
        }
        #endregion

        #region Utilitarias

        public static void EnviarMailConPdf(byte[] archivo, string email)
        {
            try
            {
                EnviarMail.EnviarMail ws = ObtenerEnvioDeMail();
                if (!ws.EnvioConAdjunto(email, "Constacia de Trámite", "", archivo, "Constancia de Trámite.pdf"))
                {
                    throw new Exception("Error en el envio de mail");
                }
            }
            catch (Exception ex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message, Environment.NewLine, ex.StackTrace), System.Diagnostics.TraceEventType.Error);
                throw new Exception("Error al intentar enviar el mail");
            }
        }

        public static string EnviarHtmlMail(string email, string idTramite, int modulo, bool envioMail)
        {
            try
            {
                ParametrosEmail param = new ParametrosEmail();
                param.Relacionados = new List<Hijo>();
                if (idTramite != null)
                {
                    param = LUPANegocio.ParametrosEmail(long.Parse(idTramite), modulo);

                    EnviarMail.EnviarMail ws = ObtenerEnvioDeMail();

                    //CONSTANCIA NUEVA
                    //Se arma la parte con los datos del tramite

                    #region Data
                    string ConstaciaHead = "<!DOCTYPE html>" +
                                            "<html lang = 'en'>" +
                                            "<head>" +
                                                "<meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />" +
                                                "<meta name='format-detection' content='telephone=no'>" +
                                                "<meta name='viewport' content='width=device-width; initial-scale=1.0; maximum-scale=1.0; user-scalable=no;'>" +
                                                "<meta http-equiv='X-UA-Compatible' content='IE=9; IE=8; IE=7; IE=EDGE' />" +
                                                "<title>Document</title>" +
                                                "<link href='https://fonts.googleapis.com/css?family=Open+Sans:400,600,700' rel='stylesheet'>" +
                                                "<style type='text/css'>*{margin: 0;padding: 0;font-family: 'Open Sans', sans-serif;font-size: 13px;border: 0;} @media screen and (max-width: 525px) { table{width: 100%;}}</style>" +
                                            "</head>" +
                                            "<body>";
                    string ConstaciaBody = "<!-- Titulo del email -->" +
                                                "<table width='600px' style='padding-left:20px;padding-right: 20px;'><tr><td style=' text-align: center;' > 'CUNA - Constancia de trámite' </td></tr></table></br>" +
                                                "<!-- Datos del titular -->";
                    #endregion

                    #region Datos titular
                    string solicitante = "";

                    // Se valida que el modulo sea decreto 614 para dejar el comprobante tal cual fue solicitado.
                    if (modulo == 35)
                    {
                        ConstaciaBody = ConstaciaBody + "<p style = 'font-weight: 600;font-size: 10px;letter-spacing: 1px;text-transform: uppercase;margin-bottom: 10px;margin-top: 10px;display: block; padding-left: 220px; ' ><strong><u> Comprobante de " + param.TipoTramite + "</u></strong><p>";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(param.Cuil))
                        {
                            solicitante = " | " + param.Cuil.Substring(0, 2) + "-" + param.Cuil.Substring(2, 8) + "-" + param.Cuil.Substring(10);
                            ConstaciaBody = ConstaciaBody + "<table class='table-titular' width='600px' style='background-color: #00519a;padding-left:20px;padding-right: 20px;margin-bottom: 10px;padding-top: 20px;padding-bottom: 20px;'><tr><td style=' text-align: center;'>" +
                                        "<p style='text-transform: uppercase;letter-spacing:2px;margin-bottom: 5px;font-weight: 600;font-size: 12px;color:rgba(255,255,255,.5);'>Titular:</p>" +
                                        "<p style='text-transform:uppercase;font-size: 15px;color: #fff;'>" + param.NombreSolicitante.ToLower() + solicitante + "</p>" +
                                    "</td></tr></table>" +
                                    "<!-- Datos del trámite-->";
                        }
                    }


                    #endregion

                    #region Datos Tramite
                    ConstaciaBody = ConstaciaBody + "<table class='text-align--left' width='600px' style='padding-left:20px;padding-right: 20px;border: 1px solid #cccc;border-radius: 3px;padding-top: 10px;padding-bottom: 10px;'>" +
                                                        "<thead><tr><td style='font-weight: 600;font-size: 10px;letter-spacing: 1px;text-transform: uppercase;margin-bottom: 10px;margin-top: 10px;display: block;'>Datos del trámite</td><td></td></tr></thead>" +
                                                        "<tbody><tr style='margin-bottom:10px;'>" +

                                                            "<!-- Número del trámite -->" +
                                                            "<td style='padding-top:5px; padding-bottom:5px;'>N° del trámite: <span style='font-weight: 600;'>" + param.numeroTramite + "</span></td>" +
                                                            "<!-- Oficina -->" +
                                                            "<td style='padding-top:5px; padding-bottom:5px;'>Oficina: <span style='font-weight: 600;text-transform: capitalize;'>" + param.UDAI + "-" + param.DenominacionUDAI.ToLower() + "</span></td>" +
                                                            "</tr>" +
                                                            "<tr>" +
                                                                "<!-- Fecha de presentación -->" +
                                                                "<td style='padding-top:5px; padding-bottom:5px;'>Fecha presentacion: <span style='font-weight: 600;'>" + param.FechaPresentacion.Substring(0, 10) + "</span></td>" +
                                                                "<!-- Estado del tramite -->" +
                                                                "<td style='padding-top:5px; padding-bottom:5px;'>Estado del trámite: <span style='font-weight: 600;text-transform: capitalize;'>" + param.estadoTramite.ToLower() + "</span></td>" +
                                                            "</tr>" +
                                                            "<tr>" +
                                                                "<!-- Novedad-->" +
                                                                "<td style='padding-top:5px; padding-bottom:5px;'>Trámite realizado: <span style='font-weight: 600;text-transform: capitalize;'>" + param.DescripcionModulo.ToLower() + "</span></td>" +
                                                                "<!-- Tipo de Novedad -->" +
                                                                "<td style='padding-top:5px; padding-bottom:5px;'>Tipo de trámite: <span style='font-weight: 600;text-transform: capitalize;'>" + param.TipoTramite.ToLower() + "</span></td>" +
                                                            "</tr>" +
                                                            "<!-- Contenido del trámite -->" +

                                                        "</tbody>" +
                                                    "</table>";

                    //Se valida si el modulo es decreto 640 para enviar el comprobante tal cual nos solicitaron    
                    if (modulo == 35 && (param.TipoTramite == "ALTA" || param.TipoTramite == "MODIFICACION"))
                    {

                        ConstaciaBody = ConstaciaBody + "<!-- Texto del mensaje -->" +
                                "<table class='text-align--left' width='600px' style=' padding-left:20px;padding-right: 20px;'><tr><td>" +
                                "<p style='margin-top: 20px;margin-bottom: 30px;font-size: 15px;line-height: 25px;'>Se ha registrado ";
                        if (param.TipoTramite == "MODIFICACION")
                        {
                            ConstaciaBody = ConstaciaBody + "una modificación en ";
                        }
                        ConstaciaBody = ConstaciaBody + "el trámite DECRETO 614/13 'Solicitud de Asignaciones para Persona a Cargo' " + (!string.IsNullOrEmpty(param.FechaVigenciaDesde) ? "con vigencia desde <strong>" + (param.FechaVigenciaDesde.Length <= 6 ? param.FechaVigenciaDesde.Substring(4) + "/" + param.FechaVigenciaDesde.Substring(0, 4) : param.FechaVigenciaDesde.Substring(0, 11)) : "") + "</strong> ";
                        if (param.Relacionados != null ? param.Relacionados.Count != 0 : false)
                        {
                            ConstaciaBody = ConstaciaBody + (!string.IsNullOrEmpty(param.Relacionados[0].Fhasta) ? ", registrando el fin del mismo en <strong> " + (param.Relacionados[0].Fhasta.Length <= 6 ? param.Relacionados[0].Fhasta.Substring(4) + "/" + param.Relacionados[0].Fhasta.Substring(0, 4) : param.Relacionados[0].Fhasta.Substring(0, 11)) : "") + "</strong>";
                        }
                        if (param.TipoTramite == "MODIFICACION")
                        {
                            ConstaciaBody = ConstaciaBody + ((param.CuilSolicitado != "" && param.CuilSolicitado != "0") ? ", mediante el cual las asignaciones que  liquiden en dicho período a <span style='text-transform: uppercase;'><strong>" + param.NombreSolicitado.ToLower() + ", CUIL: " + param.CuilSolicitado.Substring(0, 2) + "-" + param.CuilSolicitado.Substring(2, 8) + "-" + param.CuilSolicitado.Substring(10) + ".</strong></span>" : "");
                        }
                        else
                        {
                            ConstaciaBody = ConstaciaBody + ((param.CuilSolicitado != "" && param.CuilSolicitado != "0") ? ", mediante el cual las asignaciones que  liquiden a <span style='text-transform: uppercase;'><strong>" + param.NombreSolicitado.ToLower() + ", CUIL: " + param.CuilSolicitado.Substring(0, 2) + "-" + param.CuilSolicitado.Substring(2, 8) + "-" + param.CuilSolicitado.Substring(10) + ".</strong></span>" : "");
                        }
                        ConstaciaBody = ConstaciaBody + " por el/los hijo/s detallados en 'Relacionados', serán efectivizadas a <strong>" +
                                param.NombreSolicitante + " " + param.Cuil + "</strong> en el medio de pago declarado ante ANSES.";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(param.Cuil))
                        {
                            ConstaciaBody = ConstaciaBody + "<!-- Texto del mensaje -->" +
                                                        "<table class='text-align--left' width='600px' style=' padding-left:20px;padding-right: 20px;'><tr><td>" +
                                                            "<p style='margin-top: 20px;margin-bottom: 30px;font-size: 15px;line-height: 25px;'>El solicitante <span style='text-transform: uppercase;'><strong>" + param.NombreSolicitante.ToLower() + ", CUIL: " + param.Cuil + "</strong></span>";
                        }
                        else
                        {
                            ConstaciaBody = ConstaciaBody + "<table class='text-align--left' width='600px' style=' padding-left:20px;padding-right: 20px;'><tr><td>" +
                                                            "<p style='margin-top: 20px;margin-bottom: 30px;font-size: 15px;line-height: 25px;'>" + "Se";
                        }

                        ConstaciaBody = ConstaciaBody + " realizó " + (param.TipoTramite == "ALTA" ? "un <strong>Alta</strong>" : "una <span style='text-transform: capitalize;'><strong> " + param.TipoTramite.ToLower() + "</strong></span>");
                        ConstaciaBody = ConstaciaBody + (modulo == 39 ? " para el modulo <span style='text-transform: capitalize;'><strong>" + "Rehabilitación AUH por falta de Presentación Libreta " + "</strong></span>" : " para la novedad <span style='text-transform: capitalize;'><strong>" + param.DescripcionModulo.ToLower() + "</strong></span>");
                        ConstaciaBody = ConstaciaBody + ((param.CuilSolicitado != "" && param.CuilSolicitado != "0") ? "solicitando a <span style='text-transform: uppercase;'><strong>" + param.NombreSolicitado.ToLower() + "</strong></span>, CUIL: " + param.CuilSolicitado.Substring(0, 2) + "-" + param.CuilSolicitado.Substring(2, 8) + "-" + param.CuilSolicitado.Substring(10) + "." : "");
                        ConstaciaBody = ConstaciaBody + (!string.IsNullOrEmpty(param.FechaVigenciaDesde) ? "Con vigencia desde " + (param.FechaVigenciaDesde.Length <= 6 ? param.FechaVigenciaDesde.Substring(4) + "/" + param.FechaVigenciaDesde.Substring(0, 4) : param.FechaVigenciaDesde.Substring(0, 11)) : "");
                        ConstaciaBody = ConstaciaBody + (!string.IsNullOrEmpty(param.FechaVigenciaHasta) ? " Hasta " + (param.FechaVigenciaHasta.Length <= 6 ? param.FechaVigenciaHasta.Substring(4) + "/" + param.FechaVigenciaHasta.Substring(0, 4) : param.FechaVigenciaHasta.Substring(0, 11)) : "");
                        //ConstaciaBody = ConstaciaBody + (param.PeriodoPagoDesde != "" ? ". Pago desde: " + (param.PeriodoPagoDesde.Length <= 6 ? param.PeriodoPagoDesde.Substring(4) + "/" + param.PeriodoPagoDesde.Substring(0, 4) : param.PeriodoPagoDesde.Substring(3, 7)) : "");
                        //ConstaciaBody = ConstaciaBody + (param.PeriodoPagoHasta != "" ? (param.PeriodoPagoHasta.Length <= 6 ? " - Hasta: " + param.PeriodoPagoHasta.Substring(4) + "/" + param.PeriodoPagoHasta.Substring(0, 4) : " - Hasta: " + param.PeriodoPagoHasta.Substring(3, 7)) : "") + " </strong> " + "</p>";
                    }
                    #endregion

                    if (modulo == 24)
                    {
                        ListadoMDPago mdp = MDPagoDB2.TraerMedioDePagoXID(long.Parse(idTramite));
                        ConstaciaBody = ConstaciaBody + "<br>";
                        if (string.IsNullOrEmpty(mdp.CBU))
                        {
                            ConstaciaBody = ConstaciaBody + " Bco: " + mdp.Banco + " Age: " + mdp.DomicilioAgencia;
                        }
                        else
                        {
                            ConstaciaBody = ConstaciaBody + " CBU: " + mdp.CBU;
                        }
                    }

                    #region Datos de los relacionados
                    //Si es Dec840
                    if (modulo == 39)
                    {
                        ConstaciaBody = ConstaciaBody +
                            "<p> Cuil Relacionado: " + "<b>" + param.CuilRelacionado + "</b></p>" +
                            "<ul>";
                    }


                    if (param.Relacionados != null ? param.Relacionados.Count != 0 : false)
                    {
                        ConstaciaBody = ConstaciaBody +
                            "<p> Relacionados: </p>" +
                            "<ul>";

                        foreach (Hijo h in param.Relacionados)
                        {
                            var rela = "<li>" +
                                            "<p><strong>" + h.Nombre + ", CUIL: " + h.Cuil + (h.Fhasta != "0" ? (h.Fhasta != "" ? ("- Hasta: " + h.Fhasta.Substring(4) + "/" + h.Fhasta.Substring(0, 4)) : " ") : " ") + "</strong></p>" +
                                        "</li>";
                            ConstaciaBody = ConstaciaBody + rela;
                        }
                    }
                    ConstaciaBody = ConstaciaBody + "</ul>";
                    ConstaciaBody = ConstaciaBody + "</td>" +
                                                                    "</tr>" +
                                                                "</table>";
                    //"<!-- Firma-->" +
                    //"<table width='600px' style='padding-left:20px;padding-right: 20px;margin-top: 100px;'><tr><td>Firma, fecha y sello de UDAI</td><td>Firma y aclaracion del titular</td></tr></table>";
                    ConstaciaBody = ConstaciaBody + "<p style = 'text-align: left;' > &nbsp;</p>" +
                                                         "<p style = 'text-align: left;' > &nbsp;</p>";
                    #endregion

                    #region Footer
                    //si es una constacia para imprimir se le agrega lugar para firmar
                    string ConstaciaFront = ConstaciaBody +
                                                  "<div style = 'display: inline-block; border-style: solid; border-width: 1px; margin-left:10%; margin-right: 25%; width: 30%; height: 80px;'></div>" +
                                                  "<div style = 'display: inline-block; border-style: solid; border-width: 1px; margin-right: 1%; margin-bottom: 0%; width: 25%; height: 80px;'></div>" +
                                                  "<p style = 'text-align: center; display: inline-block; margin-left:10%; margin-right: 25%; width: 30%;'><strong><em> Firma, fecha y sello de la OFICINA </em></strong></p>" +
                                                  "<p style = 'text-align: center; display: inline-block; margin-right: 8%; width: 25%;'><strong><em> Firma,aclaracion del Titular </em></strong></p>";
                    string FooterMail = "<p style='text-align: center;'> &nbsp;</p>"
                                             + "<p style='text-align: left;'> Nota:</p>"
                                                  + "<p style='text-align: left;'> Los datos estan sujetos al Control de Derecho que realice ANSES.</p>"
                                                       + "<p style='text-align: left;'> La aceptacion del presente trámite no implica la liquidacion de las Asignaciones</p>";

                    #endregion

                    //Si es para mandar por mail se le agrega un footer
                    string ConstacioMail = ConstaciaHead + ConstaciaBody + FooterMail;

                    if ((param.observaciones != null) && (param.observaciones.Count > 0))
                    {
                        string Observaciones = "<p style='text-align: left;'><em> con las observaciones " + string.Join(",", param.observaciones.ToArray()).TrimEnd(',') + "</em></p>";

                        //viejo

                        if (envioMail)
                        {
                            if (!ws.EnvioUnico(email, "Constancia De Trámite", ConstacioMail, ""))
                            {
                                throw new Exception("Error en el envio de mail");
                            }
                        }
                        return ConstaciaFront;
                    }

                    if (envioMail)
                    {
                        if (!ws.EnvioUnico(email, "Constancia De Trámite", ConstacioMail, ""))
                        {
                            throw new Exception("Error en el envio de mail");
                        }
                    }
                    return ConstaciaFront;
                }
                else
                {
                    throw new Exception("ID de trámite incorrecto");
                }
            }
            catch (Exception ex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message, Environment.NewLine, ex.StackTrace), System.Diagnostics.TraceEventType.Error);
                throw new Exception("Error al intentar enviar el mail");
            }
        }

        public static string EnviarHtmlMailMiAnses(string email, string idTramite, int modulo, bool envioMail)
        {


            try
            {
                ParametrosEmail param = new ParametrosEmail();
                param.Relacionados = new List<Hijo>();
                string ConstaciaFront = "";
                if (idTramite != null)
                {
                    param = LUPANegocio.ParametrosEmail(long.Parse(idTramite), modulo);

                    EnviarMail.EnviarMail ws = ObtenerEnvioDeMail();
                    //CONSTANCIA NUEVA PARA 614

                    if (modulo == 35)
                    {
                        #region Comprobante 614

                        ConstaciaFront = "<!DOCTYPE html>" +
                                                    "<html lang = 'es'>" +
                                                    "<head>" +
                                                        "<meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />" +
                                                        "<meta name='format-detection' content='telephone=no'>" +
                                                        "<meta name='viewport' content='width=device-width; initial-scale=1.0; maximum-scale=1.0; user-scalable=no;'>" +
                                                        "<meta http-equiv='X-UA-Compatible' content='IE=9; IE=8; IE=7; IE=EDGE' />" +
                                                        "<title>Document</title>" +
                                                    "</head>";

                        ConstaciaFront = ConstaciaFront + @"<h7 style=' font-size: 12px; font-weight: normal; padding-bottom: 2em; '>Asignaciones Familiares - Constancia de trámite</h7>" +
                            @"<div class='encabezado' style=' display: flex; vertical-align: middle; flex-direction: row; flex-wrap: nowrap; align-items: center; margin-bottom: 6em; margin-top: 1em;'>
    <img src='https://www.anses.gob.ar/EmacImagenes/MiANSES/h_appmianses'  class='logo-principal'   style=' width:100%; margin-top: 2em; margin-right: 1em; position: absolute; display: block;' alt='LogoSup' tittle='LogoSup'>
</div>"
                       +
                       @"<div class='datos-tramite' style=' margin-top: 2em; '>
    <div class='info-tramite'   style='padding-top: 2em;'>
        <p style='margin-bottom: 2em; font-size: 17px;'>

            ¡Hola, <strong>" + param.NombreSolicitante + @"</strong>! <br/>
            Te informamos que se registró tu trámite de<strong> Solicitud de Asignaciones para Personas a Cargo (Decreto 614/13)</strong>, a partir de <strong>" + Convert.ToDateTime(param.FechaPresentacion).ToShortDateString() + @"</strong>.<br/>
           Las Asignaciones que se liquiden por las/los relacionadas/os al titular <strong>" + param.NombreSolicitado + @"</strong> , CUIL: <strong>" + param.CuilSolicitado + @"</strong> se harán a nombre de
            <strong>" + param.NombreSolicitante + @"</strong> , CUIL: <strong>" + param.Cuil + @"</strong> en el medio de pago declarado en <strong>ANSES</strong>.
        </p>
    </div>

    <!--relacionados-->
    <div>
        <p style='font-size: 16px;'>Personas relacionadas:</p>
    </div>";

                        foreach (var item in param.Relacionados)
                        {
                            ConstaciaFront = ConstaciaFront + "<div style=' padding: 10px; padding-left: 0px; text-transform: uppercase; font-size: 14px; font-weight: normal; font-family: Arial;'><strong>" + item.Nombre + "</strong>, Cuil: <strong> " + item.Cuil + "</strong ></div>";
                        }
                        ConstaciaFront = ConstaciaFront +
                        @"
    <!--tramite-->
        <h5 style = 'margin:10px; margin-top: 4em; font-family:Calibri;'>COMPROBANTE DE ALTA</h5>
    <div class='row datos-tramite' style=' margin-top: 1em; display: flex; flex-direction: row; flex-wrap: nowrap;'>

        <div class='col-lg-4' style=' padding: 0px !important; '>
            <div class='body-datos-2columnas' style=' border: 1px solid #9b9b9b; height: 50px;'>
                <h5 style = 'margin:10px;' ><strong> DATOS DEL TRÁMITE</strong></h5>
            </div>
            <div class='body-datos-2columnas' style=' border: 1px solid #9b9b9b; height: 50px;'>
                <label style='width: 70%; font-weight: normal; padding-left: 10px; vertical-align: central; font-size: 11px; '> N° DE TRÁMITE: <strong>" + param.numeroTramite + @"</strong></label>
            </div>
            <div class='body-datos-2columnas' style=' border: 1px solid #9b9b9b; height: 50px;'>
                <label style='width: 70%; font-weight: normal; padding-left: 10px; vertical-align: central; font-size: 11px; '> FECHA DE PRESENTACIÓN: <strong>" + Convert.ToDateTime(param.FechaPresentacion).ToShortDateString() + @"</strong></label>
            </div>
            <div class='body-datos-2columnas' style=' border: 1px solid #9b9b9b; height: 50px;'>
                <label style='width: 70%; font-weight: normal; padding-left: 10px; vertical-align: central; font-size: 11px; '> TRÁMITE REALIZADO: <strong>Titular a cargo Decreto 614</strong></label>
            </div>
        </div>
        <div class='col-lg-4'>
            <div class='body-datos-2columnas' style=' border: 1px solid #9b9b9b; height: 50px;'>
            </div>
            <div class='body-datos-2columnas' style=' border: 1px solid #9b9b9b; height: 50px;'>
                <label style='width: 70%; font-weight: normal; padding-left: 10px; vertical-align: central; font-size: 11px; '>OFICINA: <strong>" + param.UDAI + "-" + param.DenominacionUDAI + @"</strong></label>
            </div>
            <div class='body-datos-2columnas' style=' border: 1px solid #9b9b9b; height: 50px;'>
                <label style='width: 70%; font-weight: normal; padding-left: 10px; vertical-align: central; font-size: 11px; '>ESTADO DEL TRÁMITE: <strong>" + param.estadoTramite + @"</strong></label>
            </div>
            <div class='body-datos-2columnas' style=' border: 1px solid #9b9b9b; height: 50px;'>
                <label style='width: 70%; font-weight: normal; padding-left: 10px; vertical-align: central; font-size: 11px; '>TIPO DE TRÁMITE: <strong>Alta</strong></label>
            </div>
        </div>
    </div>


    <!--cuilitos-->
    <!--declaracion jurada-->
    <div class='divFooter' style='padding-top: 100px;'>
        <h4 style=' padding-bottom: 10px; font-size: 15px;'> DDJJ de Personas a Cargo:</h4>
        <p style=' font-size: 14px; '>
            'Manifiesto que tengo a mi cargo a mi/s hijo/as declarado/as en el presente trámite
            y autorizo a ANSES a que genere el derecho al cobro de las Asignaciones
            Familiares al otro/s progenitor/es y me deriven los montos liquidados por
            aplicación del Decreto N° 614/13. Asimismo, declaro bajo juramento que no existe
            a la fecha sentencia judicial que contravenga lo declarado y reconocido precedentemente;
        y que conozco por penalidades establecidas en los art. 172,292,293 y 296 Código Penal para los
        casos de falsedad en las declaraciones.'
        </p>
    </div>
</div>"
                +
                @"<hr style=' margin-top: 5em; margin-bottom: 40px;' class='pie-hr'/>

  <div class='pie' style='margin-top: 2em; vertical-align: middle;'>
    <img src='https://www.anses.gob.ar/EmacImagenes/MiANSES/f-appmianses' class='logo-footer' alt='logoFooter' title='foot2' style='width: 100%;'>
</div>";

                        #endregion


                        if (envioMail)
                        {

                            //AlternateView htmlView = AlternateView.CreateAlternateViewFromString(ConstaciaFront,null,MediaTypeNames.Text.Plain);
                            //string linkSuperior = "EmacImagenes/h-validacion.png";
                            //LinkedResource logoSuperior = new LinkedResource(linkSuperior);
                            //logoSuperior.ContentId = "logoSuperior";
                            //htmlView.LinkedResources.Add(logoSuperior);





                            if (!ws.EnvioUnico(email, "Constancia De Trámite", ConstaciaFront, ""))
                            {
                                throw new Exception("Error en el envio de mail");
                            }
                        }
                        else
                        {
                            #region Data
                            string ConstaciaHead = "<!DOCTYPE html>" +
                                                    "<html lang = 'en'>" +
                                                    "<head>" +
                                                        "<meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />" +
                                                        "<meta name='format-detection' content='telephone=no'>" +
                                                        "<meta name='viewport' content='width=device-width; initial-scale=1.0; maximum-scale=1.0; user-scalable=no;'>" +
                                                        "<meta http-equiv='X-UA-Compatible' content='IE=9; IE=8; IE=7; IE=EDGE' />" +
                                                        "<title>Document</title>" +
                                                        "<link href='https://fonts.googleapis.com/css?family=Open+Sans:400,600,700' rel='stylesheet'>" +
                                                        "<style type='text/css'>*{margin: 0;padding: 0;font-family: 'Open Sans', sans-serif;font-size: 13px;border: 0;} @media screen and (max-width: 525px) { table{width: 100%;}}</style>" +
                                                    "</head>" +
                                                    "<body>";
                            string ConstaciaBody = "<!-- Titulo del email -->" +
                                                        "<table width='600px' style='padding-left:20px;padding-right: 20px;'><tr><td style=' text-align: center;' > 'CUNA - Constancia de trámite' </td></tr></table></br>" +
                                                        "<!-- Datos del titular -->";
                            #endregion

                            #region Datos titular
                            string solicitante = "";

                            // Se valida que el modulo sea decreto 614 para dejar el comprobante tal cual fue solicitado.
                            if (modulo == 35)
                            {
                                ConstaciaBody = ConstaciaBody + "<p style = 'font-weight: 600;font-size: 10px;letter-spacing: 1px;text-transform: uppercase;margin-bottom: 10px;margin-top: 10px;display: block; padding-left: 220px; ' ><strong><u> Comprobante de " + param.TipoTramite + "</u></strong><p>";
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(param.Cuil))
                                {
                                    solicitante = " | " + param.Cuil.Substring(0, 2) + "-" + param.Cuil.Substring(2, 8) + "-" + param.Cuil.Substring(10);
                                    ConstaciaBody = ConstaciaBody + "<table class='table-titular' width='600px' style='background-color: #00519a;padding-left:20px;padding-right: 20px;margin-bottom: 10px;padding-top: 20px;padding-bottom: 20px;'><tr><td style=' text-align: center;'>" +
                                                "<p style='text-transform: uppercase;letter-spacing:2px;margin-bottom: 5px;font-weight: 600;font-size: 12px;color:rgba(255,255,255,.5);'>Titular:</p>" +
                                                "<p style='text-transform:uppercase;font-size: 15px;color: #fff;'>" + param.NombreSolicitante.ToLower() + solicitante + "</p>" +
                                            "</td></tr></table>" +
                                            "<!-- Datos del trámite-->";
                                }
                            }



                            #endregion

                            #region Datos Tramite
                            ConstaciaBody = ConstaciaBody + "<table class='text-align--left' width='600px' style='padding-left:20px;padding-right: 20px;border: 1px solid #cccc;border-radius: 3px;padding-top: 10px;padding-bottom: 10px;'>" +
                                                                "<thead><tr><td style='font-weight: 600;font-size: 10px;letter-spacing: 1px;text-transform: uppercase;margin-bottom: 10px;margin-top: 10px;display: block;'>Datos del trámite</td><td></td></tr></thead>" +
                                                                "<tbody><tr style='margin-bottom:10px;'>" +

                                                                    "<!-- Número del trámite -->" +
                                                                    "<td style='padding-top:5px; padding-bottom:5px;'>N° del trámite: <span style='font-weight: 600;'>" + param.numeroTramite + "</span></td>" +
                                                                    "<!-- Oficina -->" +
                                                                    "<td style='padding-top:5px; padding-bottom:5px;'>Oficina: <span style='font-weight: 600;text-transform: capitalize;'>" + param.UDAI + "-" + param.DenominacionUDAI.ToLower() + "</span></td>" +
                                                                    "</tr>" +
                                                                    "<tr>" +
                                                                        "<!-- Fecha de presentación -->" +
                                                                        "<td style='padding-top:5px; padding-bottom:5px;'>Fecha presentacion: <span style='font-weight: 600;'>" + param.FechaPresentacion.Substring(0, 10) + "</span></td>" +
                                                                        "<!-- Estado del tramite -->" +
                                                                        "<td style='padding-top:5px; padding-bottom:5px;'>Estado del trámite: <span style='font-weight: 600;text-transform: capitalize;'>" + param.estadoTramite.ToLower() + "</span></td>" +
                                                                    "</tr>" +
                                                                    "<tr>" +
                                                                        "<!-- Novedad-->" +
                                                                        "<td style='padding-top:5px; padding-bottom:5px;'>Trámite realizado: <span style='font-weight: 600;text-transform: capitalize;'>" + param.DescripcionModulo.ToLower() + "</span></td>" +
                                                                        "<!-- Tipo de Novedad -->" +
                                                                        "<td style='padding-top:5px; padding-bottom:5px;'>Tipo de trámite: <span style='font-weight: 600;text-transform: capitalize;'>" + param.TipoTramite.ToLower() + "</span></td>" +
                                                                    "</tr>" +
                                                                    "<!-- Contenido del trámite -->" +

                                                                "</tbody>" +
                                                            "</table>";

                            //Se valida si el modulo es decreto 640 para enviar el comprobante tal cual nos solicitaron
                            if (modulo == 35 && param.TipoTramite == "ALTA")
                            {
                                ConstaciaBody = ConstaciaBody + "<!-- Texto del mensaje -->" +
                                      "<table class='text-align--left' width='600px' style=' padding-left:20px;padding-right: 20px;'><tr><td>" +
                                        "<p style='margin-top: 20px;margin-bottom: 30px;font-size: 15px;line-height: 25px;'>Se ha registrado el trámite DECRETO 614/13 'Solicitud de Asignaciones para Persona a Cargo' " +
                                        (!string.IsNullOrEmpty(param.FechaVigenciaDesde) ? "con vigencia desde <strong>" + (param.FechaVigenciaDesde.Length <= 6 ? param.FechaVigenciaDesde.Substring(4) + "/" + param.FechaVigenciaDesde.Substring(0, 4) : param.FechaVigenciaDesde.Substring(0, 11)) : "") + "</strong> ";

                                ConstaciaBody = ConstaciaBody + ((param.CuilSolicitado != "" && param.CuilSolicitado != "0") ? ", mediante el cual las asignaciones que  liquiden a <span style='text-transform: uppercase;'><strong>" + param.NombreSolicitado.ToLower() + ", CUIL: " + param.CuilSolicitado.Substring(0, 2) + "-" + param.CuilSolicitado.Substring(2, 8) + "-" + param.CuilSolicitado.Substring(10) + ".</strong></span>" : "");

                                ConstaciaBody = ConstaciaBody + " por el/los hijo/s detallados en 'Relacionados', serán efectivizadas a <strong>" +
                                        param.NombreSolicitante + " " + param.Cuil + "</strong> en el medio de pago declarado ante ANSES.";

                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(param.Cuil))
                                {
                                    ConstaciaBody = ConstaciaBody + "<!-- Texto del mensaje -->" +
                                                                "<table class='text-align--left' width='600px' style=' padding-left:20px;padding-right: 20px;'><tr><td>" +
                                                                    "<p style='margin-top: 20px;margin-bottom: 30px;font-size: 15px;line-height: 25px;'>El solicitante <span style='text-transform: uppercase;'><strong>" + param.NombreSolicitante.ToLower() + "</strong></span>, CUIL: " + solicitante;

                                }
                                else
                                {
                                    ConstaciaBody = ConstaciaBody + "<table class='text-align--left' width='600px' style=' padding-left:20px;padding-right: 20px;'><tr><td>" +
                                                                    "<p style='margin-top: 20px;margin-bottom: 30px;font-size: 15px;line-height: 25px;'>" + "Se";
                                }
                                ConstaciaBody = ConstaciaBody + " realizó " + (param.TipoTramite == "ALTA" ? "un <strong>Alta</strong>" : "una <span style='text-transform: capitalize;'><strong> " + param.TipoTramite.ToLower() + "</strong></span>");
                                ConstaciaBody = ConstaciaBody + " para la novedad <span style='text-transform: capitalize;'><strong>" + param.DescripcionModulo.ToLower() + "</strong></span>";
                                ConstaciaBody = ConstaciaBody + ((param.CuilSolicitado != "" && param.CuilSolicitado != "0") ? "solicitando a <span style='text-transform: uppercase;'><strong>" + param.NombreSolicitado.ToLower() + "</strong></span>, CUIL: " + param.CuilSolicitado.Substring(0, 2) + "-" + param.CuilSolicitado.Substring(2, 8) + "-" + param.CuilSolicitado.Substring(10) + "." : "");
                                ConstaciaBody = ConstaciaBody + (!string.IsNullOrEmpty(param.FechaVigenciaDesde) ? "Con vigencia desde " + (param.FechaVigenciaDesde.Length <= 6 ? param.FechaVigenciaDesde.Substring(4) + "/" + param.FechaVigenciaDesde.Substring(0, 4) : param.FechaVigenciaDesde.Substring(0, 11)) : "");
                                ConstaciaBody = ConstaciaBody + (!string.IsNullOrEmpty(param.FechaVigenciaHasta) ? " Hasta " + (param.FechaVigenciaHasta.Length <= 6 ? param.FechaVigenciaHasta.Substring(4) + "/" + param.FechaVigenciaHasta.Substring(0, 4) : param.FechaVigenciaHasta.Substring(0, 11)) : "");
                                //ConstaciaBody = ConstaciaBody + (param.PeriodoPagoDesde != "" ? ". Pago desde: " + (param.PeriodoPagoDesde.Length <= 6 ? param.PeriodoPagoDesde.Substring(4) + "/" + param.PeriodoPagoDesde.Substring(0, 4) : param.PeriodoPagoDesde.Substring(3, 7)) : "");
                                //ConstaciaBody = ConstaciaBody + (param.PeriodoPagoHasta != "" ? (param.PeriodoPagoHasta.Length <= 6 ? " - Hasta: " + param.PeriodoPagoHasta.Substring(4) + "/" + param.PeriodoPagoHasta.Substring(0, 4) : " - Hasta: " + param.PeriodoPagoHasta.Substring(3, 7)) : "") + " </strong> " + "</p>";

                            }

                            #endregion

                            if (modulo == 24)
                            {
                                ListadoMDPago mdp = MDPagoDB2.TraerMedioDePagoXID(long.Parse(idTramite));
                                ConstaciaBody = ConstaciaBody + "<br>";
                                if (string.IsNullOrEmpty(mdp.CBU))
                                {
                                    ConstaciaBody = ConstaciaBody + " Bco: " + mdp.Banco + " Age: " + mdp.DomicilioAgencia;
                                }
                                else
                                {
                                    ConstaciaBody = ConstaciaBody + " CBU: " + mdp.CBU;
                                }
                            }

                            #region Datos de los relacionados
                            //Si es Dec840
                            if (modulo == 39)
                            {
                                ConstaciaBody = ConstaciaBody +
                                    "<p> Cuil Relacionado: " + "<b>" + param.CuilRelacionado + "</b></p>" +
                                    "<ul>";

                            }


                            if (param.Relacionados != null ? param.Relacionados.Count != 0 : false)
                            {
                                ConstaciaBody = ConstaciaBody +
                                    "<p> Relacionados: </p>" +
                                    "<ul>";

                                foreach (Hijo h in param.Relacionados)
                                {
                                    var rela = "<li>" +
                                                    "<strong><p>" + h.Nombre + ", CUIL: " + h.Cuil + (h.Fhasta != "0" ? (h.Fhasta != "" ? ("- Hasta: " + h.Fhasta.Substring(4) + "/" + h.Fhasta.Substring(0, 4)) : " ") : " ") + "</p></strong>" +
                                                "</li>";
                                    ConstaciaBody = ConstaciaBody + rela;
                                }
                            }
                            ConstaciaBody = ConstaciaBody + "</ul><br>";
                            //Aca va el texto 
                            ConstaciaBody = ConstaciaBody + "<p style='font-size: 14px; font-weight: bold; font-style:italic;'>''DDJJ de Personas a Cargo:</p>" +
                                                              "<span style='font-style:italic;font-size: 13px;'>Manifiesto que tengo a mi cargo a mi/s hijo/s declarado/s en el presente trámite y " +
                                                              "autorizo a ANSES a que genere el derecho al cobro de las Asignaciones Familiares " +
                                                              "al otro/s progenitor/es y me deriven los montos liquidados por aplicación del Decreto" +
                                                              "N° 614/13. Asimismo, declaro bajo juramento que no existe a la fecha sentencia" +
                                                              " judicial que contravenga lo declarado y reconocido precedentemente; y que conozco las" +
                                                              " penalidades establecidas en los art. 172,292,293 y 296 Código Penal para los casos" +
                                                              " de falsedad en las declaraciones.''</span>";
                            ConstaciaBody = ConstaciaBody + "</td>" +
                                                                            "</tr>" +
                                                                        "</table>";
                            #endregion

                            #region Footer

                            //si es una constacia para imprimir se le agrega lugar para firmar
                            //ESTO ESTABA ANTES, LE PEGA EL CUADRADITO DE FIRMA Y ACLARACION.
                            /*string ConstaciaFront = ConstaciaBody +
                                                          "<div style = 'display: inline-block; border-style: solid; border-width: 1px; margin-left:10%; margin-right: 25%; width: 30%; height: 80px;'></div>" +
                                                          "<div style = 'display: inline-block; border-style: solid; border-width: 1px; margin-right: 1%; margin-bottom: 0%; width: 25%; height: 80px;'></div>" +
                                                          "<p style = 'text-align: center; display: inline-block; margin-left:10%; margin-right: 25%; width: 30%;'><strong><em> Firma, fecha y sello de UDAI </em></strong></p>" +
                                                          "<p style = 'text-align: center; display: inline-block; margin-right: 8%; width: 25%;'><strong><em> Firma,aclaracion del Titular </em></strong></p>";*/

                            ConstaciaFront = ConstaciaBody;

                            string FooterMail = "<p style='text-align: center;'> &nbsp;</p>"
                                                     + "<p style='text-align: left;'> Nota:</p>"
                                                          + "<p style='text-align: left;'> Los datos estan sujetos al Control de Derecho que realice ANSES.</p>"
                                                               + "<p style='text-align: left;'> La aceptacion del presente trámite no implica la liquidacion de las Asignaciones</p>";

                            #endregion

                            //Si es para mandar por mail se le agrega un footer
                            string ConstacioMail = ConstaciaHead + ConstaciaBody + FooterMail;
                            #endregion



                            if ((param.observaciones != null) && (param.observaciones.Count > 0))
                            {
                                string Observaciones = "<p style='text-align: left;'><em> con las observaciones " + string.Join(",", param.observaciones.ToArray()).TrimEnd(',') + "</em></p>";

                                //viejo

                                if (envioMail)
                                {
                                    if (!ws.EnvioUnico(email, "Constancia De Trámite", ConstacioMail, ""))
                                    {
                                        throw new Exception("Error en el envio de mail");
                                    }
                                }
                                return ConstaciaFront;
                            }

                            if (envioMail)
                            {
                                if (!ws.EnvioUnico(email, "Constancia De Trámite", ConstacioMail, ""))
                                {
                                    throw new Exception("Error en el envio de mail");
                                }
                            }
                            return ConstaciaFront;
                        }


                    }
                    else
                    {
                        throw new Exception("ID de trámite incorrecto");
                    }
                }
                return ConstaciaFront;
            }
            catch (Exception ex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message, Environment.NewLine, ex.StackTrace), System.Diagnostics.TraceEventType.Error);
                throw new Exception("Error al intentar enviar el mail");
            }
        }
        public static List<Hijo> leerXmlNombreCuilito(string sxmlstring)
        {
            AdministradorDeLog.LogTexto(string.Format("leerXmlNombreCuilito-1 Xml: {0}", sxmlstring), System.Diagnostics.TraceEventType.Information);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sxmlstring);

            XmlNodeList hijos = doc.DocumentElement.SelectNodes("/Root/Hijo");

            List<Hijo> hijosRelacionados = new List<Hijo>();

            foreach (XmlNode hijo in hijos)
            {

                XmlNode cuil = hijo.SelectSingleNode("Cuil");
                XmlNode fhasta = hijo.SelectSingleNode("Fhasta");

                Hijo hijoRelacionado = new Hijo();

                hijoRelacionado.Cuil = cuil.InnerXml;
                hijoRelacionado.Fhasta = fhasta.InnerText;

                AdministradorDeLog.LogTexto(string.Format("leerXmlNombreCuilito-2 hijoCuil: {0} hijoFhasta: {0}", hijoRelacionado.Cuil, hijoRelacionado.Fhasta), System.Diagnostics.TraceEventType.Information);
                try
                {
                    hijoRelacionado.Nombre = PersonaNegocio.TraerDatosPersonaxCuil(hijoRelacionado.Cuil).PersonaCuip.ape_nom;
                    AdministradorDeLog.LogTexto(string.Format("leerXmlNombreCuilito-3 hijoNombre: {0}", hijoRelacionado.Nombre), System.Diagnostics.TraceEventType.Information);
                }
                catch (Exception ex)
                {
                    AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message), System.Diagnostics.TraceEventType.Error);
                    hijoRelacionado.Nombre = "Error al consultar ADP";
                }
                hijosRelacionados.Add(hijoRelacionado);
            }
            return hijosRelacionados;
        }
        public static List<string> LeerXmlObservaciones(string xmlObservaciones)
        {
            List<string> obs = new List<string>();
            XmlDocument doc = new XmlDocument();
            if (!string.IsNullOrEmpty(xmlObservaciones))
            {
                doc.LoadXml(xmlObservaciones);

                XmlNodeList observaciones = doc.DocumentElement.SelectNodes("/root");

                foreach (XmlNode o in observaciones)
                {
                    XmlNode observaci = o.SelectSingleNode("Observacion");
                    if (observaci != null)
                    {
                        obs.Add(observaci.InnerText);
                    }
                }
            }
            return obs;
        }
        public static bool LeerXmlCronoliq(string Param)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Param);

            XmlNodeList Preliquidacion = doc.DocumentElement.SelectNodes("/cronoliq/pre-liquidacion");

            foreach (XmlNode o in Preliquidacion)
            {
                var atributos = o.Attributes;
                /* XmlNode estado = o.SelectSingleNode("estado");
                 XmlNode fin = o.SelectSingleNode("fin");
                 XmlNode inicio = o.SelectSingleNode("inicio");*/

                if (atributos != null)
                {
                    if ((atributos[2].Value == "" || atributos[2].Value == "erroneo") && atributos[0].Value == "")
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public static bool EsCuilProvisorio(string cuil)
        {
            decimal DNI = decimal.Parse(cuil.Substring(2, 8));
            return (DNI >= 60000000 && DNI <= 69999999);
        }
        public static bool EsRelacionValidaRenunciaDeposito(short CodigoRelacion, string Descripcion)
        {
            return (CodigoRelacion == 1 || //CONYUGE
                    CodigoRelacion == 2 || //CONVIVIENTE
                   (CodigoRelacion == 3 && Descripcion == "HIJO/A") || //HIJO
                    CodigoRelacion == 4 || //GUARDA
                    CodigoRelacion == 5 || //TENENCIA
                    CodigoRelacion == 6 || //TUTELA
                    CodigoRelacion == 11 || //HIJO ADOPCION PLENA
                    CodigoRelacion == 12 || //HIJO ADOPCION SIMPLE
                    CodigoRelacion == 14 || //PERSONA A CARGO AAFF
                    CodigoRelacion == 19 || //CURATELA A LAS PERSONAS
                    CodigoRelacion == 22 || //TUTELA LEGAL
                    CodigoRelacion == 23 || //TENENCIA CON RENUNCIA
                    CodigoRelacion == 25 || //CONVIVIENTE PREVISIONAL
                    CodigoRelacion == 30 || //A CARGO ASIGN. UNIVERSAL
                    CodigoRelacion == 31); //UVHI MDS
        }
        public static bool EsRelacionValidaDerechoHabiente(short CodigoRelacion, string Descripcion)
        {
            return ((CodigoRelacion == 3 && Descripcion == "HIJO/A") || //HIJO
                    CodigoRelacion == 4 || //GUARDA
                    CodigoRelacion == 5 || //TENENCIA
                    CodigoRelacion == 6 || //TUTELA
                    CodigoRelacion == 11 || //HIJO ADOPCION PLENA
                    CodigoRelacion == 12 || //HIJO ADOPCION SIMPLE
                    CodigoRelacion == 19 || //CURATELA A LAS PERSONAS
                    CodigoRelacion == 23);    //TENENCIA CON RENUNCIA
        }
        public static RelacionesDH TienePrenaMat(adpWS.RetornoDatosPersonaCuip fallecido, string cuilFallecido)
        {
            RelacionesDH rel = new RelacionesDH();
            if (fallecido.PersonaCuip.sexo == "F" || fallecido.PersonaCuip.sexo == "f")
            {
                List<Prenatal> prenatales = new List<Prenatal>();
                Prenatal prenatal = new Prenatal();
                List<Maternidad> maternidades = new List<Maternidad>();
                Maternidad maternidad = new Maternidad();

                prenatales = PrenatalNegocio.PrenatalLista(long.Parse(cuilFallecido));
                prenatal = prenatales.Find(p => p.FechaPresentacion <= fallecido.PersonaCuip.f_actu_falle && p.AptoBaja == true && p.AptoAcreditacion == true && p.FechaHastaPago >= DateTime.Today);
                if (prenatal != null)
                {
                    rel.Prenatal = prenatal;
                    rel.TienePrenatal = true;
                }

                maternidades = MaternidadNegocio.maternidadLista(long.Parse(cuilFallecido));
                maternidad = maternidades.Find(m => m.FechaPresentacion <= fallecido.PersonaCuip.f_actu_falle && m.AptoBaja == true && m.PeriodoPagoHasta >= ((DateTime.Today).Year + DateTime.Today.Month));
                if (maternidad != null)
                {
                    rel.Maternidad = maternidad;
                    rel.TieneMaternidad = true;
                }
            }
            return rel;
        }
        public static bool FallecidoPeriodoDesde(decimal? periodoDesde, DateTime fechaFalle, short codigoFalle)
        {
            bool fallecido = false;
            if (periodoDesde != 0 && periodoDesde.HasValue)
            {
                if (fechaFalle != new DateTime())
                {
                    fallecido = FechaAPeriodo(fechaFalle) < periodoDesde.Value;
                }
            }
            return fallecido;
        }
        public static bool RelacionVigentePeDesde(Wspw04.DatosPw04 relacion, decimal? periodoDesde)
        {
            bool vigente = false;
            if (periodoDesde != 0 && periodoDesde.HasValue)
            {
                vigente = FechaAPeriodo(FormatDateSinBarras(relacion.f_desde)) <= periodoDesde && (relacion.f_vig_hasta != "" ? FechaAPeriodo(FormatDateSinBarras(relacion.f_vig_hasta)) >= periodoDesde : true);
            }
            return vigente;
        }

        #region formateoDatos
        public static DateTime PeriodoAFecha(decimal Periodo)
        {
            try
            {
                return DateTime.Parse("01/" + Periodo.ToString().Substring(4) + "/" + Periodo.ToString().Substring(0, 4));
            }
            catch
            {
                return new DateTime();
            }
        }
        public static decimal FechaAPeriodo(DateTime Fecha)
        {
            try
            {
                return Fecha.Year * 100 + Fecha.Month;
            }
            catch
            {
                return 0;
            }
        }
        public static DateTime FormatDateSinBarras(string Date)
        {
            try
            {
                DateTime fecha = new DateTime();
                if (Date.Length == 8)
                {
                    DateTime.TryParse(Date.Substring(0, 2) + "/" + Date.Substring(2, 2) + "/" + Date.Substring(4), out fecha);
                }
                return fecha;
            }
            catch
            {
                return new DateTime();
            }
        }
        public static string FormatearFechaTipoStingddMMyyyy(string fecha)
        {
            try
            {
                return fecha.Length != 8 ? null : (fecha.Substring(0, 2) + "/" + fecha.Substring(2, 2) + "/" + fecha.Substring(4));
            }
            catch
            {
                return "";
            }
        }
        public static string PasarDeCodigoASistema(short CodigoDeSistema)
        {
            try
            {
                return (CodigoDeSistema == 69 ? "CUNA" : CodigoDeSistema == 60 ? "UVHI" : "SUAF");
            }
            catch
            {
                return "";
            }
        }
        public static bool ValidarMail(string mail)
        {

            try
            {
                MailAddress m = new MailAddress(mail);

                return true;
            }
            catch (Exception)
            {
                return false;
            }


        }
        public static bool EsTelefonoValido(string numero, string EsCel, int telediscado, int telediscadoPais)
        {
            try
            {
                bool EsNumeroValido = long.Parse(numero) > 100000 && NumerosDistintos(numero);
                return (EsNumeroValido && EsCel == "S" && telediscado > 0);
            }
            catch
            {
                return false;
            }
        }
        public static bool NumerosDistintos(string telefono)
        {
            try
            {
                string numeroAnte;
                bool sonIguales = true;
                var numeros = telefono.ToCharArray().ToList();
                numeroAnte = numeros.First().ToString();
                numeros.ForEach(x =>
                {
                    if (!x.ToString().Equals(numeroAnte))
                    {
                        sonIguales = false;
                    }
                    numeroAnte = x.ToString();
                });
                return !sonIguales;
            }
            catch
            {
                return false;
            }
        }
        #endregion


        private static string buscarInfoEnToken(string clave, SSOToken token)
        {

            foreach (var item in token.Operation.Login.Info)
            {
                if (item.Name.ToLower().Equals(clave.ToLower()))
                {
                    return item.Value;
                }
            }
            return "";
        }

        public static AuditoriaConsulta GetAuditoria(string token)
        {

            // CON EL TOKEN OBTENGO LAS CREDENCIALES
            SSOEncodedToken ssoEncodedToken = new SSOEncodedToken()
            {
                Token = token
            };
            AuditoriaConsulta ret = new AuditoriaConsulta();


            // DtoToken token = new DtoToken();
            try
            {

                var MiToken = Credencial.ObtenerCredencialEnWs(ssoEncodedToken);
                //CON LA CREDENCIAL OBTENGO LOS VALORES QUE NECESITO PARA CREAR AuditoriaConsulta

                ret.Operador = MiToken.Operation.Login.UId;
                ret.IpOrigen = buscarInfoEnToken("ip", MiToken);
                string udai = buscarInfoEnToken("oficina", MiToken);
                if (!String.IsNullOrEmpty(udai))
                    ret.UDAI = Convert.ToDecimal(udai);


            }
            catch (Exception e)
            {

                AdministradorDeLog.LogTexto(e.Message, System.Diagnostics.TraceEventType.Information);
                ret.Operador = "NOGET";
                ret.IpOrigen = "0.0";
                ret.UDAI = 0;
            }
            return ret;
        }
    }
}
