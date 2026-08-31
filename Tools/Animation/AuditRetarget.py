"""Independently audit an SF2 retarget against the model and donor clip (stdlib)."""
import argparse
import hashlib
import json
import math
from pathlib import Path
import struct
import xml.etree.ElementTree as ET


def read(path):
    data=path.read_bytes()
    frames=[]
    offset=4
    for _ in range(struct.unpack_from('<i',data)[0]):
        count=struct.unpack_from('<i',data,offset+1)[0]
        offset+=5
        frames.append([struct.unpack_from('<3f',data,offset+12*i) for i in range(count)])
        offset+=12*count
    assert offset==len(data), 'Unconsumed data'
    return frames


def audit(animation,model,reference,reference_frame=3):
    xml=ET.parse(model).getroot()
    nodes=list(xml.find('Nodes'))
    names=[n.tag for n in nodes]
    frames=read(animation)
    ref=dict(zip(names,read(reference)[reference_frame]))
    assert frames and len(names)==67
    lengths=[]
    for side in ('1','2'):
        lengths.extend([(a+'_'+side,b+'_'+side) for a,b in [
            ('NShoulder','NElbow'),('NElbow','NWrist'),('NHip','NKnee'),('NKnee','NAnkle'),
            ('NAnkle','NToe'),('NAnkle','NHeel'),('NToe','NToeTip'),('NToe','NToeS'),
            ('NWrist','NKnuckles'),('NKnuckles','NFingertips')]])
    lengths.extend([('NPivot','NStomach'),('NStomach','NChest'),('NChest','NNeck'),('NNeck','NHead'),('NHead','NTop')])
    largest=0
    macro_error=0
    com_error=0
    previous=None
    max_step=0
    masses={n.tag:float(n.attrib.get('Mass',0)) for n in nodes}
    for frame in frames:
        assert len(frame)==67
        assert all(math.isfinite(v) for p in frame for v in p)
        p=dict(zip(names,frame))
        for a,b in lengths:
            error=abs(math.dist(p[a],p[b])-math.dist(ref[a],ref[b]))
            largest=max(largest,error)
            assert error<0.001, ('Segment stretched',a,b,error)
        floor=min(p[n+'_'+s][1] for s in ('1','2') for n in ('NToe','NToeTip','NHeel','NToeS'))
        assert abs(floor)<0.001, ('Not grounded',floor)
        assert abs(p['NPivot'][2])<0.001, 'Root left the fighting plane'
        for node in nodes:
            if node.attrib['Type']=='MacroNode':
                expected=[sum(p[node.attrib['ChildNode'+str(j)]][axis]*float(node.attrib['LCC'+str(j)]) for j in range(1,int(node.attrib['NodesCount'])+1)) for axis in range(3)]
                error=math.dist(expected,p[node.tag])
                macro_error=max(macro_error,error)
                assert error<0.001, ('Macro node mismatch',node.tag)
            if node.attrib['Type']=='CenterOfMass':
                children=[node.attrib['ChildNode'+str(j)] for j in range(1,int(node.attrib['NodesCount'])+1)]
                mass=sum(masses[c] for c in children)
                expected=[sum(p[c][axis]*masses[c] for c in children)/mass for axis in range(3)]
                error=math.dist(expected,p[node.tag])
                com_error=max(com_error,error)
                assert error<0.001, 'COM mismatch'
        if previous:
            max_step=max(max_step,max(math.dist(p[n],previous[n]) for n in names if not n.startswith('Weapon-Node')))
        previous=p
    # Fast fighting-game keyframes may legitimately move farther than a limb
    # length in one tick. Report this for source comparison; do not smooth or
    # reject solely on an arbitrary displacement threshold.
    return {'status':'PASS','frames':len(frames),'nodes':len(names),'file_bytes':animation.stat().st_size,
            'max_segment_length_error':largest,'max_macro_error':macro_error,'max_com_error':com_error,
            'max_node_step_per_sample':max_step,
            'warnings':['Fast sample transition: compare with source motion before combat use.'] if max_step>100 else [],
            'sha256':hashlib.sha256(animation.read_bytes()).hexdigest()}


def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument('animation',type=Path)
    p.add_argument('--model',type=Path,required=True)
    p.add_argument('--reference',type=Path,required=True)
    p.add_argument('--reference-frame',type=int,default=3)
    p.add_argument('--report',type=Path)
    a=p.parse_args()
    result=audit(a.animation,a.model,a.reference,a.reference_frame)
    text=json.dumps(result,indent=2)+'\n'
    if a.report:
        a.report.write_text(text)
    print(text)


if __name__=='__main__':
    main()
