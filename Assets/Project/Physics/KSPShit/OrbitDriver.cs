﻿namespace Experimental
{
    using System;
    using UnityEngine;

    public class OrbitDriver : MonoBehaviour
    {
        public enum UpdateMode
        {
            PLANET,
            VESSEL,
            VESSEL_ACTIVE
        }

        public delegate void CelestialBodyDelegate(CelestialBody body);

        public Vector3d pos;
        public Vector3d vel;

        public Vector3 startVel;
        public Vector3 localCoM;
        public Vector3 CoMoffset;

        private bool isHyperbolic;

        public Orbit orbit = new Orbit();

        public bool drawOrbit;
        public bool reverse;
        public bool frameShift;
        public bool QueuedUpdate;

        public UpdateMode updateMode;

        //public OrbitRenderer Renderer;

        private bool ready;

        public Vessel vessel;

        public CelestialBody celestialBody;

        public Color orbitColor = Color.grey;

        public Transform driverTransform;

        public CelestialBodyDelegate OnReferenceBodyChange;

        public CelestialBody referenceBody
        {
            get
            {
                return orbit.referenceBody;
            }
            set
            {
                orbit.referenceBody = value;
            }
        }

        private void Awake()
        {
            driverTransform = transform;
            vessel = GetComponent<Vessel>();

            if(celestialBody == null) celestialBody = GetComponent<CelestialBody>();
        }

        public void ro()
        {
            if (vessel != null && referenceBody != null)
                orbit = Orbit.CreateRandomOrbitAround(referenceBody);
        }

        private void Start()
        {
            switch (updateMode)
            {
                case UpdateMode.VESSEL:
                    if (referenceBody == null)
                    {
                        referenceBody = FlightGlobals.getMainBody(driverTransform.position);
                    }
                    TrackRigidbody(referenceBody);
                    break;
                case UpdateMode.VESSEL_ACTIVE:
                    if (referenceBody == null)
                    {
                        referenceBody = FlightGlobals.getMainBody(driverTransform.position);
                    }
                    TrackRigidbody(referenceBody);
                    break;
                case UpdateMode.PLANET:
                    orbit.Init();
                    updateFromParameters();
                    break;
            }

            ready = true;

            Planetarium.Orbits.Add(this);

            if (OnReferenceBodyChange != null)
            {
                OnReferenceBodyChange(referenceBody);
            }
        }

        private void OnDestroy()
        {
            if (Planetarium.Orbits == null) return;

            Planetarium.Orbits.Remove(this);
        }

        private void FixedUpdate()
        {
            if (QueuedUpdate) return;

            UpdateOrbit();
        }

        public void UpdateOrbit()
        {
            if (!ready) return;

            switch (updateMode)
            {
                case UpdateMode.VESSEL:
                    if (vessel != null)
                    {
                        if (vessel.rb != null)
                        {
                            TrackRigidbody(referenceBody);
                        }

                        CheckDominantBody(driverTransform.position);
                    }
                    break;
                case UpdateMode.VESSEL_ACTIVE:
                    if (vessel != null)
                    {
                        if (vessel.rb != null)
                        {
                            TrackRigidbody(referenceBody);
                        }

                        CheckDominantBody(driverTransform.position);
                    }
                    //updateFromParametersForVessel();
                    break;
                case UpdateMode.PLANET:
                    if (vessel != null)
                    {
                        CheckDominantBody(referenceBody.position + pos);
                    }
                    updateFromParameters();
                    break;
            }
            if (isHyperbolic && orbit.eccentricity < 1.0)
            {
                isHyperbolic = false;
                if (vessel != null)
                {
                    //GameEvents.onVesselOrbitClosed.Fire(vessel);
                }
            }
            if (!isHyperbolic && orbit.eccentricity > 1.0)
            {
                isHyperbolic = true;
                if (vessel != null)
                {
                    //GameEvents.onVesselOrbitEscaped.Fire(vessel);
                }
            }

            Debug.DrawRay(driverTransform.position, orbit.vel.xzy * 100 * Time.deltaTime, (updateMode != UpdateMode.PLANET) ? Color.red : Color.cyan); //TimeWarp.fixedDeltaTime

            if (drawOrbit)
            {
                orbit.DrawOrbit();
            }
        }

        public void SetOrbitMode(UpdateMode mode)
        {
            updateMode = mode;
        }

        private void CheckDominantBody(Vector3d refPos)
        {
            //if (referenceBody != FlightGlobals.getMainBody(refPos) && !FlightGlobals.overrideOrbit)
            if (referenceBody != FlightGlobals.getMainBody(refPos))
            {
                RecalculateOrbit(FlightGlobals.getMainBody(refPos));
            }
        }

        private void TrackRigidbody(CelestialBody refBody)
        {
            localCoM = vessel.findLocalCenterOfMass();

            //pos = ((Vector3d)(driverTransform.position + driverTransform.rotation * localCoM - (Vector3)refBody.position)).xzy;
            pos = ((Vector3d)((driverTransform.position + driverTransform.rotation * localCoM - (Vector3)refBody.position)) - (Vector3d)driverTransform.position).xzy;

            if (updateMode == UpdateMode.VESSEL)
            {
                vel = orbit.GetRotFrameVel(referenceBody);
            }

            if (vessel.rb != null && !vessel.rb.isKinematic)
            {
                //vel = vessel.rootPart.rb.GetPointVelocity(driverTransform.TransformPoint(localCoM));// + Krakensbane.GetFrameVelocity();
                vel = vessel.velocity;// + vessel.rb.GetPointVelocity(driverTransform.TransformPoint(localCoM));
                vel = vel.xzy + orbit.GetRotFrameVel(referenceBody);
            }

            vel = vel + referenceBody.GetFrameVel() - refBody.GetFrameVel();
            pos += vel * Time.fixedDeltaTime;
            orbit.UpdateFromStateVectors(pos, vel, refBody, Planetarium.GetUniversalTime());
        }

        public void updateFromParameters()
        {
            orbit.UpdateFromUT(Planetarium.GetUniversalTime());

            pos = orbit.pos.xzy;
            vel = orbit.vel;

            if (double.IsNaN(pos.x))
            {
                if (vessel)
                {
                    //Shit happens...
                }
            }
            if (!reverse)
            {
                if (vessel)
                {
                    //CoMoffset = driverTransform.rotation * localCoM;
                    vessel.SetPosition(referenceBody.position + pos - (Vector3d)CoMoffset);
                }
                else if (celestialBody)
                {
                    celestialBody.position = referenceBody.position + pos;
                }
                else
                {
                    driverTransform.position = referenceBody.position + pos;
                }
            }
            else
            {
                referenceBody.position = ((!celestialBody) ? (Vector3d)driverTransform.position : celestialBody.position) - pos;
            }
        }

        public void RecalculateOrbit(CelestialBody newReferenceBody)
        {
            if (frameShift) return;

            frameShift = true;

            //CelestialBody referenceBody = referenceBody;
            if (updateMode == UpdateMode.PLANET && Time.timeScale > 0f)
            {
                OnRailsSOITransition(orbit, newReferenceBody);
            }
            else
            {
                TrackRigidbody(newReferenceBody);
                orbit.epoch = Planetarium.GetUniversalTime() - Time.fixedDeltaTime; //(double)TimeWarp.fixedDeltaTime;
            }

            if (OnReferenceBodyChange != null)
            {
                OnReferenceBodyChange(newReferenceBody);
            }

            if (vessel != null)
            {
                //GameEvents.onVesselSOIChanged.Fire(new GameEvents.HostedFromToAction<Vessel, CelestialBody>(vessel, referenceBody, newReferenceBody));
                //ModuleTripLogger.ForceVesselLog(vessel, FlightLog.EntryType.Flyby, newReferenceBody);
            }

            Invoke("unlockFrameSwitch", 0.5f);
        }

        public void OnRailsSOITransition(Orbit ownOrbit, CelestialBody to)
        {
            double universalTime = Planetarium.GetUniversalTime();
            double vMin = universalTime - 1.0 * 1.0;//(double)TimeWarp.CurrentRate;
            double SOIsqr = 0.0;

            if (orbit.referenceBody.HasChild(to))
            {
                SOIsqr = to.sphereOfInfluence * to.sphereOfInfluence;
                BSPSolver(ref universalTime, 1.0 * 1.0, (double t) => Math.Abs((ownOrbit.getPositionAtUT(t) - to.getPositionAtUT(t)).sqrMagnitude - SOIsqr), vMin, universalTime, 0.01, 64); //(double)TimeWarp.CurrentRate
            }
            else if (to.HasChild(orbit.referenceBody))
            {
                SOIsqr = orbit.referenceBody.sphereOfInfluence * orbit.referenceBody.sphereOfInfluence;
                BSPSolver(ref universalTime, 1.0 * 1.0, (double t) => Math.Abs(ownOrbit.getRelativePositionAtUT(t).sqrMagnitude - SOIsqr), vMin, universalTime, 0.01, 64); //(double)TimeWarp.CurrentRate
            }

            ownOrbit.UpdateFromOrbitAtUT(ownOrbit, universalTime, to);
        }

        private void unlockFrameSwitch()
        {
            frameShift = false;
        }

        public static int BSPSolver(ref double v0, double dv, Func<double, double> solveFor, double vMin, double vMax, double epsilon, int maxIterations)
        {
            int result;

            if (v0 < vMin)
            {
                result = 0;
            }
            else
            {
                if (v0 > vMax)
                {
                    result = 0;
                }
                else
                {
                    int num = 0;
                    double num2 = solveFor(v0);
                    while (dv > epsilon && num2 < maxIterations)
                    {
                        double num3 = solveFor(v0 + dv);
                        double num4 = solveFor(v0 - dv);

                        if (v0 - dv < vMin)
                        {
                            num4 = 1.7976931348623157E+308;
                        }

                        if (v0 + dv > vMax)
                        {
                            num3 = 1.7976931348623157E+308;
                        }

                        num3 = Math.Abs(num3);
                        num4 = Math.Abs(num4);
                        num2 = Math.Min(num2, Math.Min(num3, num4));

                        if (num2 == num4)
                        {
                            v0 -= dv;
                        }
                        else
                        {
                            if (num2 == num3)
                            {
                                v0 += dv;
                            }
                        }

                        dv /= 2.0;
                        num++;
                    }
                    result = num;
                }
            }
            return result;
        }
    }
}